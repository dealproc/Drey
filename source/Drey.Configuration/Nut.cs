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
        ServiceModel.HoardeManager _hoardeManager;
        [NonSerialized]
        ServiceModel.IServicesManager _servicesManager;

        /// <summary>
        /// Gets the version of the dll for display on the web console.
        /// </summary>
        public static string Version
        {
            get { return typeof(Nut).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; }
        }

        public override string Id
        {
            get { return "Drey.Configuration"; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return true; }
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }

        public override bool Startup(INutConfiguration configurationManager)
        {
            if (!base.Startup(configurationManager)) { return false; }

            try
            {
                MigrationManager.Migrate(configurationManager);
            }
            catch (Exception ex)
            {
                _log.FatalException("Migration Failed.", ex);
                return false;
            }

            configurationManager.CertificateValidator.Initialize();

            _eventBus = new EventBus();
            _hoardeManager = new ServiceModel.HoardeManager(_eventBus, configurationManager, ShellRequestHandler, this.ConfigureLogging);

            Infrastructure.IoC.AutofacConfig.Configure(_eventBus, _hoardeManager, configurationManager);

            _eventBus.Subscribe(this);

            _servicesManager = Infrastructure.IoC.AutofacConfig.Container.Resolve<ServiceModel.IServicesManager>();
            _servicesManager.Start();

            var startupUri = string.Format("http://localhost:{0}/", ConfigurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Bootstrapper(), new[] { new Uri(startupUri) });
            host.Start();

            _webApp = host;

            return true;
        }

        public override void Shutdown()
        {
            Log.InfoFormat("{id} is shutting down.", this.Id);

            _hoardeManager.Dispose();
            _webApp.Dispose();
            _webApp = null;
        }

        public void Handle(ShellRequestArgs message)
        {
            if (message.PackageId.Equals(this.Id, StringComparison.OrdinalIgnoreCase))
            {
                EmitShellRequest(message);
            }
        }

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