using Drey.Logging;
using Drey.Nut;
using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Drey
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlPanelServiceControl : MarshalByRefObject, IDisposable
    {
        readonly ILog _log;
        readonly ShellFactory _appFactory;
        readonly INutConfiguration _nutConfiguration;
        readonly ExecutionMode _executionMode;
        readonly Action<INutConfiguration> _configureLogging;

        Tuple<AppDomain, IShell> _console;

        Task _restartConsoleTask;
        ShellRequestArgs _requestArgsForRestart;

        bool _disposed = false;

        public ControlPanelServiceControl(ExecutionMode mode = ExecutionMode.Production, Action<INutConfiguration> configureLogging = null)
        {
            _nutConfiguration = new ApplicationHostNutConfiguration { Mode = mode };
            _executionMode = mode;

            if (configureLogging != null)
            {
                _configureLogging = configureLogging;
                _configureLogging.Invoke(_nutConfiguration);
            }

            _log = LogProvider.For<ControlPanelServiceControl>();
            _appFactory = new ShellFactory();
        }
        ~ControlPanelServiceControl()
        {
            Dispose(false);
        }

        public bool Start()
        {
            return StartupConsole();
        }

        public bool Stop()
        {
            ShutdownConsole();
            return true;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Handles requests coming from an app instance.
        /// <remarks>
        /// Realistically, the only app that will utilize this event will be Drey.Configuration, so it can request for instances to be started/stopped/restarted as necessary.
        /// </remarks>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void ShellRequestHandler(object sender, ShellRequestArgs e)
        {
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
                    default:
                        _log.ErrorFormat("Received an unknown action: {0}", e.ActionToTake);
                        break;
                }

                return;
            }

            _console.Item2.ShellRequestHandler(sender, e);
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

            var dir = new System.IO.DirectoryInfo(Drey.Utilities.PathUtilities.MapPath(_nutConfiguration.HoardeBaseDirectory));
            var deployments = dir.EnumerateDirectories(e.PackageId + "*", searchOption: System.IO.SearchOption.TopDirectoryOnly)
                .Where(di => !di.Name.EndsWith(e.Version))
                .Apply(di =>
                {
                    _log.DebugFormat("Removing {folder} from hoarde.", di.Name);
                    Directory.Delete(di.FullName, true); // This removes all contents of the folder as well.
                });
        }
        
        private bool StartupConsole()
        {
            _log.Info("Console is starting up.");
            string packageDir = Utilities.PackageUtils.DiscoverPackage(DreyConstants.ConfigurationPackageName, _nutConfiguration.HoardeBaseDirectory);
            _console = _appFactory.Create(packageDir, ShellRequestHandler, _configureLogging);
            
            return _console.Item2.Startup(_nutConfiguration);
        }

        void ShutdownConsole()
        {
            if (_console == null)
            {
                _log.Debug("Console has been shutdown already.");
                return;
            }


            try
            {
                _log.Info("Console is shutting down.");

                if (_console.Item2 != null)
                {
                    _console.Item2.Shutdown();
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
            }

            _log.Info("Console has shutdown.");
        }

        void ObserveConsoleFinalizationAndRestart()
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

            _disposed = true;
        }
    }
}
