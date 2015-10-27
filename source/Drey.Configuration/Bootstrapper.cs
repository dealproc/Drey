using Drey.Configuration.Repositories.SQLiteRepositories;
using Drey.Logging;
using Drey.Nut;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Embedded.Conventions;
using Nancy.TinyIoc;
using Nancy.Validation.DataAnnotations;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Drey.Configuration
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        static readonly ILog _log = LogProvider.For<Bootstrapper>();

        readonly INutConfiguration _configurationManager;
        readonly Assembly ThisAssembly;
        readonly IEventBus _eventBus;
        ServiceModel.IServicesManager _servicesManager;
        ServiceModel.HoardeManager _hoardeManager;

        public Bootstrapper(ServiceModel.HoardeManager hoardeManager, IEventBus eventBus, INutConfiguration configurationManager) : base()
        {
            _log.Trace("Bootstrapper has been constructed for Drey.Configuration.");

            _hoardeManager = hoardeManager;
            _eventBus = eventBus;
            _configurationManager = configurationManager;

            ThisAssembly = this.GetType().Assembly;

            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            _log.Debug("Configuration of the application container has happened.");

            base.ConfigureApplicationContainer(container);

            container.Register<IEventBus>(_eventBus);
            container.Register<ServiceModel.HoardeManager>(_hoardeManager);
            container.Register<INutConfiguration>(_configurationManager);

            // we need to manually register the data annotations registrations because we are
            // loading the *.dlls in a dedicated app domain.
            container.Register(new DataAnnotationsRegistrations());

            container.Register<ServiceModel.PollingClientCollection>().AsSingleton();
            container.Register<ServiceModel.RegisteredPackagesPollingClient>().AsSingleton();

            container.Register<Repositories.IGlobalSettingsRepository, GlobalSettingsRepository>();
            switch (_configurationManager.Mode)
            {
                case ExecutionMode.Development:
                    container.Register<Repositories.IPackageRepository, Repositories.OnDisk.OnDiskPackageRepository>();
                    break;
                case ExecutionMode.Production:
                    container.Register<Repositories.IPackageRepository, Repositories.SQLiteRepositories.PackageRepository>();
                    break;
                default:
                    container.Register<Repositories.IPackageRepository, Repositories.SQLiteRepositories.PackageRepository>();
                    _log.WarnFormat("Unknown execution mode '{mode}'.  Registered Sqlite Repository.", _configurationManager.Mode);
                    break;
            }

            container.Register<Services.IGlobalSettingsService, Services.GlobalSettingsService>();

            container.Register<ServiceModel.IServicesManager, ServiceModel.ServicesManager>().AsSingleton();

            _servicesManager = container.Resolve<ServiceModel.IServicesManager>();
            _hoardeManager = container.Resolve<ServiceModel.HoardeManager>();

            _servicesManager.Start();
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder); }
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }

        protected override IEnumerable<Type> ModelValidatorFactories
        {
            get { yield return typeof(DataAnnotationsValidatorFactory); }
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            _log.Trace("Conventions are being set for resolving embedded assets.");
            if (!ResourceViewLocationProvider.RootNamespaces.ContainsKey(ThisAssembly))
            {
                ResourceViewLocationProvider.RootNamespaces.Add(ThisAssembly, this.GetType().Namespace + ".Views");
                base.ConfigureConventions(conventions);
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Content", ThisAssembly));
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/fonts", ThisAssembly));
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Scripts", ThisAssembly));
            }
        }

        protected override IEnumerable<Type> ViewEngines
        {
            get { yield return typeof(RazorViewEngine); }
        }
    }
}