using Drey.Logging;
using Drey.Nut;

using System;
using System.Security.Permissions;

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
            if (e.PackageId.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase))
            {
                _log.InfoFormat("'Shell Request' Event Received: {0}", e);

                switch (e.ActionToTake)
                {
                    case ShellAction.Startup:
                        _log.Warn("Seems odd to get a startup call here.  Should investigate.");
                        break;
                    case ShellAction.Shutdown:
                        ShutdownConsole();
                        break;
                    case ShellAction.Restart:
                        ShutdownConsole();
                        StartupConsole();
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
            string packageDir = Utilities.PackageUtils.DiscoverPackage(DreyConstants.ConfigurationPackageName, _nutConfiguration.HoardeBaseDirectory);
            var shell = _appFactory.Create(packageDir, ShellRequestHandler, _configureLogging);
            shell.Item2.Startup(_nutConfiguration);
            return true;
        }

        void ShutdownConsole()
        {
            try
            {
                _console.Item2.Shutdown();
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
        }
    }
}
