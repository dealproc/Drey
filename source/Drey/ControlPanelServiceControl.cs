using Drey.Logging;
using Drey.Nut;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Topshelf;

namespace Drey
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlPanelServiceControl : MarshalByRefObject, ServiceControl, IDisposable
    {
        readonly ILog _log;
        readonly Sponsor<ShellFactory> _appFactory;
        readonly INutConfiguration _configurationManager;
        readonly ExecutionMode _executionMode;
        readonly Action<INutConfiguration> _configureLogging;

        Tuple<AppDomain, Sponsor<IShell>> _console;

        Task _restartConsoleTask;
        ShellRequestArgs _requestArgsForRestart;

        bool _disposed = false;

        bool _shuttingDown = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPanelServiceControl"/> class.
        /// </summary>
        /// <param name="mode">
        /// Which mode is the system running in?
        /// <list type="table">
        /// <item><term>Production</term><description>Drey will use its normal package detail information to load packages into the runtime for execution.</description></item>
        /// <item><term>Development</term><description>Drey will discover packages in the ~/Hoarde folder and load them for execution. This prevents the need to package your system for every build.</description></item>
        /// </list>
        /// </param>
        /// <param name="configureLogging">An action that gets run at the startup of every package which configures the logging provider for that package.</param>
        /// <param name="logVerbosity">How much logging should be captured?  Pass a compatible string here that works with your chosen framework, and parse it within the configureLogging method.</param>
        public ControlPanelServiceControl(ExecutionMode mode = ExecutionMode.Production, Action<INutConfiguration> configureLogging = null, string logVerbosity = "Info")
        {
            _configurationManager = new ApplicationHostNutConfiguration() { Mode = mode, LogVerbosity = logVerbosity };
            _executionMode = mode;

            if (configureLogging != null)
            {
                _configureLogging = configureLogging;
                _configureLogging.Invoke(_configurationManager);
            }

            _log = LogProvider.For<ControlPanelServiceControl>();
            _appFactory = new Sponsor<ShellFactory>(new ShellFactory());
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="ControlPanelServiceControl"/> class.
        /// </summary>
        ~ControlPanelServiceControl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public bool Start(HostControl hostControl)
        {
            var started = StartupConsole();
            _log.InfoFormat("Startup was successful? {started}", started);
            return started;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public bool Stop(HostControl hostControl)
        {
            _shuttingDown = true;
            ShutdownConsole();
            return true;
        }

        /// <summary>
        /// Handles requests coming from an app instance.
        /// <remarks>
        /// Realistically, the only app that will utilize this event will be Drey.Configuration, so it can request for instances to be started/stopped/restarted as necessary.
        /// </remarks>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ShellRequestHandler(object sender, ShellRequestArgs e)
        {
            // when shutting down, the shutdown command is executing the ShutdownConsole() method.
            if (_shuttingDown) { return; }

            // right now, the shutdown command is working as expected, but cannot seem to trigger a restart.
            if (e.PackageId.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase))
            {
                _log.InfoFormat("Control Panel 'Shell Request' Event Received: {0}", e);

                switch (e.ActionToTake)
                {
                    case ShellAction.Startup:
                        _log.Warn("Seems odd to get a startup call here.  Should investigate.");
                        break;
                    case ShellAction.Shutdown:
                        ShutdownConsole();
                        break;
                    case ShellAction.Restart:
                        if (_restartConsoleTask == null)
                        {
                            _restartConsoleTask = new Task(ObserveConsoleFinalizationAndRestart);
                            _restartConsoleTask.Start();
                        }
                        _requestArgsForRestart = e;
                        ShutdownConsole();
                        break;
                    case ShellAction.Heartbeat:
                        _log.TraceFormat("Received heartbeat from {packageId} at {now}", e.PackageId, DateTime.Now);
                        break;
                    default:
                        _log.ErrorFormat("Received an unknown action: {0}", e.ActionToTake);
                        break;
                }

                return;
            }

            //_console.Item2.Protege.ShellRequestHandler(sender, e);
        }

        /// <summary>
        /// Removes historical versions of the applet from the hoarde.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void CleanupHoarde(ShellRequestArgs e)
        {
            _log.InfoFormat("Remove other versions: {removeVersions} | Action to Take: {action}", e.RemoveOtherVersionsOnRestart, e.ActionToTake);

            if (!(e.RemoveOtherVersionsOnRestart && e.ActionToTake == ShellAction.Restart)) { return; }

            _log.Info("Cleaning up hoarde due to restart and RemoveOtherVersionsOnRestart being set to true.");

            var dir = new DirectoryInfo(Utilities.PathUtilities.MapPath(_configurationManager.HoardeBaseDirectory));
            var deployments = dir.EnumerateDirectories(e.PackageId + "*", searchOption: System.IO.SearchOption.TopDirectoryOnly)
                .Where(di => !di.Name.EndsWith(e.Version))
                .Apply(di =>
                {
                    _log.DebugFormat("Removing {folder} from hoarde.", di.Name);
                    Directory.Delete(di.FullName, true); // This removes all contents of the folder as well.
                });
        }

        /// <summary>
        /// Brings the Drey.Console package online, so other packages can then be brought online or taken offline on demand.
        /// </summary>
        /// <returns></returns>
        private bool StartupConsole()
        {
            _log.Info("Console is starting up.");
            string packageDir = Utilities.PackageUtils.DiscoverPackage(DreyConstants.ConfigurationPackageName, _configurationManager.HoardeBaseDirectory);
            _log.InfoFormat("Configuration Package located at '{packageDir}'", packageDir);

            _console = _appFactory.Protege.Create(packageDir, ShellRequestHandler, _configureLogging, Path.Combine(_configurationManager.PluginsBaseDirectory, DreyConstants.ConfigurationPackageName));
            _log.InfoFormat("Starting console service.");

            return _console.Item2.Protege.Startup(_configurationManager);
        }

        /// <summary>
        /// Shuts down the Drey Console and all packages running within it, and subsequently cleans up its app domain.
        /// </summary>
        private void ShutdownConsole()
        {
            if (_console == null)
            {
                _log.Debug("Console has been shutdown already.");
                return;
            }

            try
            {
                _log.Info(new string('*', 50));
                _log.Info(new string('*', 50));
                _log.Info("Console is shutting down.");

                if (_console.Item2 != null)
                {
                    _console.Item2.Protege.Shutdown();
                    _console.Item2.Dispose();
                }
                else
                {
                    _log.Debug("Applet shell is empty.  Must have been shutdown.");
                }

                _log.Debug("Unloading console app domain.");

                AppDomain.Unload(_console.Item1);
            }
            catch (CannotUnloadAppDomainException ex)
            {
                _log.WarnException("Could not unload app domain.", ex);
            }
            catch (AppDomainUnloadedException)
            {
                _log.Info("App domain has already been unloaded.");
            }
            finally
            {
                _console = null;

                _log.Info("Console has shutdown.");
                _log.Info(new string('*', 50));
                _log.Info(new string('*', 50));
            }
        }

        /// <summary>
        /// Observes the console finalization and restart.
        /// <remarks>Due to app domains being shutdown, we needed to provide a higher level tool to observe the shut-down of the app domain before we startup a new instance (with regard to the control panel).</remarks>
        /// </summary>
        private void ObserveConsoleFinalizationAndRestart()
        {
            _log.Info("Waiting for console to shutdown so it can be restarted.");
            bool restartIssued = false;

            while (!restartIssued)
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                if (_console == null)
                {
                    _restartConsoleTask = null;
                    _log.Info("Console has completed its shutdown process... restarting.");
                    StartupConsole();
                    restartIssued = true;

                    if (_requestArgsForRestart != null)
                    {
                        _log.Info("Attempting to cleanup old versions of the console in the hoarde.");
                        CleanupHoarde(_requestArgsForRestart);
                    }

                    _requestArgsForRestart = null;
                }
            }

            _restartConsoleTask = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            _log.Debug("Control Panel Service Control is being disposed.");

            if (_restartConsoleTask != null)
            {
                _log.Debug("Disposing the restart console task");
                _restartConsoleTask.Dispose();
                _restartConsoleTask = null;
            }

            if (_appFactory != null)
            {
                _appFactory.Dispose();
            }

            _disposed = true;
        }
    }
}
