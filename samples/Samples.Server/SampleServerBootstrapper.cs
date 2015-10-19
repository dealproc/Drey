using Autofac;

using Nancy.Bootstrappers.Autofac;

namespace Samples.Server
{
    public class SampleServerBootstrapper : AutofacNancyBootstrapper
    {
        readonly IContainer _container;

        public SampleServerBootstrapper(IContainer container)
        {
            new Drey.Server.Exceptions.RuntimeHasNotConnectedException();
            _container = container;
        }
        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }
        protected override System.Collections.Generic.IEnumerable<Nancy.INancyModule> GetAllModules(ILifetimeScope container) {
            var mod = base.GetAllModules(container);
            return mod;
        }
    }
}