using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Permissions;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Manages all packages loaded into the runtime.
    /// </summary>
    public class HoardeManager : MarshalByRefObject, IHandle<ShellRequestArgs>, IHoardeManager
    {
        static ILog _log = LogProvider.For<HoardeManager>();

        readonly IEventBus _eventBus;
        readonly ShellFactory _appFactory;
        readonly INutConfiguration _configurationManager;
        readonly Action<INutConfiguration> _configureLogging;

        ConcurrentDictionary<Guid, Tuple<AppDomain, IShell>> _apps;
        EventHandler<ShellRequestArgs> _shellRequestHandler;

        bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HoardeManager"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="shellRequestHandler">The shell request handler.</param>
        /// <param name="configureLogging">The configure logging.</param>
        public HoardeManager(IEventBus eventBus, INutConfiguration configurationManager, EventHandler<ShellRequestArgs> shellRequestHandler, Action<INutConfiguration> configureLogging)
        {
            _eventBus = eventBus;
            _appFactory = new ShellFactory();
            _configurationManager = configurationManager;
            _shellRequestHandler = shellRequestHandler;
            _configureLogging = configureLogging;

            _apps = new ConcurrentDictionary<Guid, Tuple<AppDomain, IShell>>();

            _eventBus.Subscribe(this);
        }
        ~HoardeManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Handles a ShellRequestArgs event.
        /// </summary>
        /// <param name="e">The e.</param>
        public void Handle(ShellRequestArgs e)
        {
            _log.InfoFormat("'Shell Request' Event Received: {packageId} | {event}", e.PackageId, e.ActionToTake);

            if (e.PackageId.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase))
            {
                _log.DebugFormat("Hoarde manager is not responsible for {packageId}.", DreyConstants.ConfigurationPackageName);
                return;
            }

            switch (e.ActionToTake)
            {
                case ShellAction.Startup:
                    StartupInstance(e.ConfigurationManager, e.PackageId, e.Version);
                    break;
                case ShellAction.Shutdown:
                    ShutdownInstance(e.PackageId);
                    break;
                case ShellAction.Restart:
                    ShutdownInstance(e.PackageId);
                    CleanupHoarde(e);
                    StartupInstance(e.ConfigurationManager, e.PackageId, e.Version);
                    break;
                default:
                    _log.ErrorFormat("Received an unknown action: {0}", e.ActionToTake);
                    break;
            }
        }

        /// <summary>
        /// Determines whether the specified package is loaded and executing in the runtime.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        public bool IsOnline(DataModel.Release package)
        {
            _log.DebugFormat("Online packages: {packageList}", _apps.Select(x => x.Value.Item2.Id).ToArray());

            var isOnline = _apps.Any(a => a.Value.Item2.Id.Equals(package.Id, StringComparison.OrdinalIgnoreCase));
            
            _log.DebugFormat("Is {package} online? {isOnline}", package.Id, isOnline);
            
            return isOnline;
        }

        /// <summary>
        /// Starts an app instance from the horde.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="version">The version. <remarks>If no version is passed into this method, the system will try to identify the latest version of the app, and start that version.</remarks></param>
        /// <returns></returns>
        bool StartupInstance(INutConfiguration configurationManager, string id, string version = "")
        {
            string packageDir = string.IsNullOrWhiteSpace(version)
                ?
                Utilities.PackageUtils.DiscoverPackage(id, _configurationManager.HoardeBaseDirectory)
                :
                Utilities.PackageUtils.DiscoverPackage(id, _configurationManager.HoardeBaseDirectory, version);

            var shell = _appFactory.Create(packageDir, _shellRequestHandler, _configureLogging, Path.Combine(_configurationManager.PluginsBaseDirectory, id));
            if (shell == null)
            {
                _log.Fatal("Did not create the configuration console.  app exiting.");
                return false;
            }


            _log.InfoFormat("Starting {app}", shell.Item2.Id);

            if (shell.Item2.Startup(configurationManager))
            {
                _apps.TryAdd(Guid.NewGuid(), shell);
            }
            else
            {
                _log.InfoFormat("{app} failed to start.  Shutting app down.", shell.Item2.Id);
                KillAppContainer(shell);
            }

            _log.Info("Configuration shell created.  Starting to listen for events.");
            return true;
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
            var deployments = dir.EnumerateDirectories(e.PackageId + "*", searchOption: SearchOption.TopDirectoryOnly)
                .Where(di => !di.Name.EndsWith(e.Version))
                .Apply(di =>
                {
                    _log.DebugFormat("Removing {folder} from hoarde.", di.Name);
                    Directory.Delete(di.FullName, true); // This removes all contents of the folder as well.
                });
        }

        /// <summary>
        /// Shutdowns an app instance, based on its package id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void ShutdownInstance(string id)
        {
            _log.DebugFormat("Attempting to remove {id}", id);

            var instancesToShutdown = _apps.Where(i => i.Value.Item2.Id == id).Select(x => x.Key).ToArray();
            foreach (var key in instancesToShutdown)
            {
                KillAppContainer(_apps[key]);
                Tuple<AppDomain, IShell> removed;
                if (_apps.TryRemove(key, out removed))
                {
                    _log.Debug("App domain details have been removed from hoarde successfully.");
                }
                else
                {
                    _log.Debug("Removal of app domain details from hoarde did not happen.");
                }

            }
        }
        
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Kills the application container.
        /// </summary>
        /// <param name="shell">The shell.</param>
        private void KillAppContainer(Tuple<AppDomain, IShell> shell)
        {
            try
            {
                shell.Item2.Shutdown();
                _log.Debug("Shutdown has completed.");
                shell.Item2.Dispose();
                _log.Debug("Shell has been disposed.");
                AppDomain.Unload(shell.Item1);
            }
            catch (CannotUnloadAppDomainException ex)
            {
                _log.WarnException("Could not unload app domain.", ex);
            }
            catch (AppDomainUnloadedException ex)
            {
                _log.WarnException("failure to unload app domain?", ex);
            }
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

            var appIds = _apps.Select(x => x.Value.Item2.Id).ToArray();

            appIds.Apply(ShutdownInstance);

            _disposed = true;
        }
    }
}
