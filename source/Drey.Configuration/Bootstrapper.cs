using Autofac;

using Drey.Configuration.Repositories.SQLiteRepositories;
using Drey.Logging;
using Drey.Nut;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
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
    /// <summary>
    /// The nancy bootstrapper.
    /// </summary>
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        static readonly ILog _log = LogProvider.For<Bootstrapper>();

        readonly Assembly ThisAssembly;

        public Bootstrapper() : base()
        {
            _log.Trace("Bootstrapper has been constructed for Drey.Configuration.");
            ThisAssembly = this.GetType().Assembly;
            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return Infrastructure.IoC.AutofacConfig.Container;
        }

        protected override void ApplicationStartup(Autofac.ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            Nancy.Security.Csrf.Enable(pipelines);
        }

        protected override void ConfigureApplicationContainer(Autofac.ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);

            // we need to manually register the data annotations registrations because we are
            // loading the *.dlls in a dedicated app domain.

            var builder = new ContainerBuilder();
            builder.RegisterType<DataAnnotationsRegistrations>().AsImplementedInterfaces();

            builder.Update(Infrastructure.IoC.AutofacConfig.Container);
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