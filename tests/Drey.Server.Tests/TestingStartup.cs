using Nancy.Bootstrapper;
using Owin;
using System;

namespace Drey.Server.Tests
{
    class TestingStartup
    {
        public static Func<INancyBootstrapper> BootstrapperFunc = () => null;

        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new Nancy.Owin.NancyOptions
            {
                Bootstrapper = BootstrapperFunc.Invoke(),
                EnableClientCertificates = true
            });
        }
    }
}