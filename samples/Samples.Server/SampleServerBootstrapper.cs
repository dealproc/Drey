using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using System;

namespace Samples.Server
{
    public class SampleServerBootstrapper : AutofacNancyBootstrapper
    {
        readonly IContainer _container;

        public SampleServerBootstrapper(IContainer container)
        {
            new Drey.Server.Exceptions.RuntimeHasNotConnectedException();
            _container = container;
            StaticConfiguration.DisableErrorTraces = false;
            //StaticConfiguration.AllowFileStreamUploadAsync = false;
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.OnError += (ctx, ex) =>
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }
        protected override Nancy.Diagnostics.DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new Nancy.Diagnostics.DiagnosticsConfiguration { Password = "1234" };
            }
        }
    }
}