using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Embedded.Conventions;
using Nancy.Extensions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Drey.Configuration
{
    public class Bootstrapper : DefaultNancyBootstrapper, IHandle<Infrastructure.Events.RecycleApp>
    {
        readonly Drey.Nut.INutConfiguration _configurationManager;
        readonly Assembly ThisAssembly;
        readonly IEventBus _eventBus;

        public Bootstrapper(Drey.Nut.INutConfiguration configurationManager, IEventBus eventBus)
            : base()
        {
            _configurationManager = configurationManager;
            _eventBus = eventBus;

            ThisAssembly = this.GetType().Assembly;
            _eventBus.Subscribe(this);
        }
        ~Bootstrapper()
        {
            _eventBus.Unsubscribe(this);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            InitializePollingClients();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<ServiceModel.PollingClientCollection>().AsSingleton();
            container.Register<ServiceModel.RegisteredPackagesPollingClient>().AsSingleton();

            container.Register<Drey.Nut.INutConfiguration>(_configurationManager);

            container.Register<Repositories.IGlobalSettingsRepository, Repositories.SQLiteRepositories.GlobalSettingsRepository>();

            container.Register<Services.IGlobalSettingsService, Services.GlobalSettingsService>();

            container.Register<IEventBus>(_eventBus);
        }

        protected override Nancy.Bootstrapper.NancyInternalConfiguration InternalConfiguration
        {
            get { return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder); }
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
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

        public void Handle(Infrastructure.Events.RecycleApp message)
        {
            InitializePollingClients();
        }

        private void InitializePollingClients()
        {
            var globalSettingsService = ApplicationContainer.Resolve<Services.IGlobalSettingsService>();
            if (globalSettingsService.HasValidSettings())
            {
                var registeredPackagesPoller = ApplicationContainer.Resolve<ServiceModel.RegisteredPackagesPollingClient>();
                var pollingCollection = ApplicationContainer.Resolve<ServiceModel.PollingClientCollection>();
                pollingCollection.Add(registeredPackagesPoller);
            }
        }
    }
}