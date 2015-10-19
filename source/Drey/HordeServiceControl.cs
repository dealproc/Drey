using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace Drey
{
    /// <summary>
    /// This is a non-platform specific implementation of the service control class.
    /// <remarks>
    /// <para>This should be created within the platform specific service and subsequently started/stopped using the platform-specific start/stop methods.</para>
    /// <para>This class inherits from <see cref="MarshalByRefObject" /> because we need to cross app domain boundaries.</para>
    /// </remarks>
    /// </summary>
    public class HordeServiceControl : MarshalByRefObject
    {
        public static Action<INutConfiguration> ConfigureLogging = (config) => { };

        readonly ILog _log;
        readonly ShellFactory _appFactory;
        readonly INutConfiguration _nutConfiguration;
        readonly List<Tuple<AppDomain, IShell>> _appInstances;

        public HordeServiceControl()
        {
            _log = LogProvider.For<HordeServiceControl>();
            _appFactory = new ShellFactory();
            _nutConfiguration = new ApplicationHostNutConfiguration();
            _appInstances = new List<Tuple<AppDomain, IShell>>();
        }

        /// <summary>
        /// Starts the horde service.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            ConfigureLogging(_nutConfiguration);
            _log.Info("Runtime is starting.");
            return StartupInstance(_nutConfiguration, DreyConstants.ConfigurationPackageName, string.Empty);
        }

        /// <summary>
        /// Stops the horde service.
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _log.Info("Runtime is shutting down.");

            var packageIds = _appInstances.Select(x => x.Item2.Id).ToArray();
            packageIds.Apply(ShutdownInstance);
            _appInstances.Clear();

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
            _log.InfoFormat("'Shell Request' Event Received: {0}", e);

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
                    StartupInstance(e.ConfigurationManager, e.PackageId, e.Version);
                    break;
                default:
                    _log.ErrorFormat("Received an unknown action: {0}", e.ActionToTake);
                    break;
            }
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
                Utilities.PackageUtils.DiscoverPackage(id, _nutConfiguration.HordeBaseDirectory)
                :
                Utilities.PackageUtils.DiscoverPackage(id, _nutConfiguration.HordeBaseDirectory, version);

            var shell = _appFactory.Create(packageDir, configurationManager);
            if (shell == null)
            {
                _log.Fatal("Did not create the configuration console.  app exiting.");
                return false;
            }

            _log.Info("Configuration shell created.  Starting to listen for events.");
            shell.Item2.OnShellRequest += ShellRequestHandler;
            _appInstances.Add(shell);
            return true;
        }

        /// <summary>
        /// Shutdowns an app instance, based on its package id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void ShutdownInstance(string id)
        {
            var instancesToShutdown = _appInstances.Where(i => i.Item2.Id == id).ToArray();
            instancesToShutdown.Apply(i =>
            {
                try
                {
                    i.Item2.Shutdown();
                    AppDomain.Unload(i.Item1);
                }
                finally
                {
                    _appInstances.Remove(i);
                }
            });
        }
    }
}