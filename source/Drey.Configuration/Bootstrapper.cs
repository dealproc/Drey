using Nancy;
using Nancy.Bootstrapper;
using Nancy.Embedded.Conventions;
using Nancy.ViewEngines;
using System.Reflection;

namespace Drey.Configuration
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        readonly Assembly ThisAssembly;

        public Bootstrapper() : base()
        {
            ThisAssembly = this.GetType().Assembly;
        }

        protected override Nancy.Bootstrapper.NancyInternalConfiguration InternalConfiguration
        {
            get{return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder);}
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions conventions)
        {
            ResourceViewLocationProvider.RootNamespaces.Add(ThisAssembly, this.GetType().Namespace + ".Views");
            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Content", ThisAssembly));
            conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/fonts", ThisAssembly));
            conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Scripts", ThisAssembly));
        }
    }
}