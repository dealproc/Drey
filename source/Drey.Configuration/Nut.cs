using Autofac;

using Drey.Configuration.Infrastructure.Schema;
using Drey.Logging;
using Drey.Nut;

using Nancy.Hosting.Self;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Drey.Configuration
{
    [Serializable]
    public class Nut : ShellBase, IHandle<ShellRequestArgs>, IDisposable
    {
        static ILog _log = LogProvider.For<Nut>();

        IEventBus _eventBus;
        IDisposable _webApp;
        
        [NonSerialized]
        ServiceModel.IHoardeManager _hoardeManager;
        
        [NonSerialized]
        ServiceModel.IServicesManager _servicesManager;

        /// <summary>
        /// Gets the version of the dll for display on the web console.
        /// </summary>
        public static string Version
        {
            get { return typeof(Nut).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; }
        }

        /// <summary>
        /// Gets the package identifier.
        /// </summary>
        public override string Id
        {
            get { return "Drey.Configuration"; }
        }

        /// <summary>
        /// Gets a value indicating whether this package requires use of the configuration storage provided by the runtime.
        /// </summary>
        public override bool RequiresConfigurationStorage
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the default application settings for this package.
        /// </summary>
        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        /// <summary>
        /// Gets the default connection string list for this package.
        /// </summary>
        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }

        /// <summary>
        /// Startups the specified host configuration MGR.
        /// </summary>
        /// <param name="hostConfigMgr">The host configuration MGR.</param>
        /// <returns></returns>
        public override bool Startup(INutConfiguration hostConfigMgr)
        {
            if (!base.Startup(hostConfigMgr)) { return false; }
            if (!MigrateDatabase(hostConfigMgr)) { return false; }
            if (string.IsNullOrWhiteSpace(hostConfigMgr.ApplicationSettings["drey.configuration.consoleport"]))
            {
                _log.Fatal("Console port is missing or not set.  Cannot start.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(hostConfigMgr.ApplicationSettings["WorkingDirectory"]))
            {
                _log.Fatal("Hoarde working directory is missing or has not been configured.  Cannot start.");
                return false;
            }

            SetupIoCContainer(hostConfigMgr);
            ResetConfigurationManager();

            try
            {
                _log.Debug("Attempting to set certificate validation within the runtime env.");
                ConfigurationManager.CertificateValidator.Initialize();
            }
            catch (Exception ex)
            {
                _log.WarnException("While trying to configure certificate validation", ex);
            }

            try {
                StartServicesManager();
                StartWebConsole(hostConfigMgr);
            }
            catch (Exception exc)
            {
                _log.FatalException("Could not bring web console online", exc);
                return false;
            }

            return true;
        }
        private static bool MigrateDatabase(INutConfiguration configurationManager)
        {
            try
            {
                MigrationManager.Migrate(configurationManager);
                return true;
            }
            catch (Exception ex)
            {
                var baseEx = ex.GetBaseException();
                _log.FatalException("Base exception of migration failure:", ex);
                _log.FatalException("Migration Failed.", ex);
                return false;
            }
        }
        private void SetupIoCContainer(INutConfiguration configurationManager)
        {
            _eventBus = new EventBus();
            _eventBus.Subscribe(this);
            _hoardeManager = new ServiceModel.HoardeManager(_eventBus, configurationManager, ShellRequestHandler, this.ConfigureLogging);
            Infrastructure.IoC.AutofacConfig.Configure(_eventBus, _hoardeManager, configurationManager);
        }
        private void ResetConfigurationManager()
        {
            if (this.ConfigurationManager is Infrastructure.ConfigurationManagement.DbConfigurationSettings)
            {
                _log.Info("Configuration manager has already been reset.");
                return;
            }

            var pkgRepository = Infrastructure.IoC.AutofacConfig.Container.Resolve<Repositories.IPackageRepository>();
            var pkgSettingsRepository = Infrastructure.IoC.AutofacConfig.Container.Resolve<Repositories.IPackageSettingRepository>();
            var connStrRepository = Infrastructure.IoC.AutofacConfig.Container.Resolve<Repositories.IConnectionStringRepository>();

            this.ConfigurationManager = new Infrastructure.ConfigurationManagement.DbConfigurationSettings(this.ConfigurationManager, pkgSettingsRepository, connStrRepository, this.Id);
        }
        private void StartServicesManager()
        {
            _servicesManager = Infrastructure.IoC.AutofacConfig.Container.Resolve<ServiceModel.IServicesManager>();
            _servicesManager.Start();
        }
        private void StartWebConsole(INutConfiguration configMgr)
        {
            var startupUri = string.Format("http://localhost:{0}/", configMgr.ApplicationSettings["drey.configuration.consoleport"]);
            _log.InfoFormat("Bringing web console online at {uri}", startupUri);
            var host = new NancyHost(new Bootstrapper(), new[] { new Uri(startupUri) });
            host.Start();

            _webApp = host;
        }

        /// <summary>
        /// Should your app need a specific shutdown routine, you will override this method and impement it.
        /// </summary>
        public override void Shutdown()
        {
            Log.InfoFormat("{id} is shutting down.", this.Id);

            if (_hoardeManager != null)
            {
                _hoardeManager.Dispose();
                _hoardeManager = null;
            }

            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }
        }

        /// <summary>
        /// Handles the ShellRequestArgs message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ShellRequestArgs message)
        {
            if (message.PackageId.Equals(this.Id, StringComparison.OrdinalIgnoreCase))
            {
                EmitShellRequest(message);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            Log.Trace("Disposing Drey Configuration host.");

            base.Dispose(disposing);

            if (!disposing) { return; }

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(this);
                _eventBus = null;
            }

            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }

            if (_hoardeManager != null)
            {
                _hoardeManager.Dispose();
                _hoardeManager = null;
            }

            if (_servicesManager != null)
            {
                _servicesManager.Stop();
                _servicesManager = null;
            }

            Infrastructure.IoC.AutofacConfig.DisposeContainer();
        }
    }
}