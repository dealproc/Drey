using Drey.Logging;
using Drey.Nut;

using System;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Drey
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlPanelServiceControl : MarshalByRefObject
    {
        readonly ILog _log;
        readonly ShellFactory _appFactory;
        readonly INutConfiguration _nutConfiguration;
        readonly ExecutionMode _executionMode;
        readonly Action<INutConfiguration> _configureLogging;

        Tuple<AppDomain, IShell> _console;

        Task _restartConsoleTask;

        public ControlPanelServiceControl(ExecutionMode mode = ExecutionMode.Production, Action<INutConfiguration> configureLogging = null)
        {
            if (configureLogging != null)
            {
                _configureLogging = configureLogging;
                _configureLogging.Invoke(_nutConfiguration);
            }

            _log = LogProvider.For<ControlPanelServiceControl>();
            _appFactory = new ShellFactory();
            _nutConfiguration = new ApplicationHostNutConfiguration { Mode = mode };
            _executionMode = mode;
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
                        _restartConsoleTask.Start();
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

        private bool StartupConsole()
        {
            _log.Info("Console is starting up.");
            string packageDir = Utilities.PackageUtils.DiscoverPackage(DreyConstants.ConfigurationPackageName, _nutConfiguration.HoardeBaseDirectory);
            _console = _appFactory.Create(packageDir, ShellRequestHandler, _configureLogging);
            _console.Item2.Startup(_nutConfiguration);

            if (_restartConsoleTask == null)
            {
                _restartConsoleTask = new Task(ObserveConsoleFinalizationAndRestart);
            }

            return true;
        }

        void ShutdownConsole()
        {
            _log.Info("Console is shutting down.");

            try
            {
                _console.Item2.Shutdown();
                _log.Debug("Unloading console app domain.");
                AppDomain.Unload(_console.Item1);
            }
            catch (CannotUnloadAppDomainException ex)
            {
                _log.WarnException("Could not unload app domain.", ex);
            }
            catch (AppDomainUnloadedException ex)
            {
                _log.WarnException("Failure to unload app domain.", ex);
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
                }
            }

        }
    }
}
