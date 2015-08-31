using Owin;

namespace Drey.Server.Tests
{
    class TestingStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new Nancy.Owin.NancyOptions
            {
                EnableClientCertificates = true
            });
        }
    }
}