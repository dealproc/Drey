using Nancy;
using Nancy.Bootstrapper;
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

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            ResourceViewLocationProvider.RootNamespaces.Add(ThisAssembly, this.GetType().Namespace + ".Views");
            base.ConfigureConventions(nancyConventions);
        }
    }
}