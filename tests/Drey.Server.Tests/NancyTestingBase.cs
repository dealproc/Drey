using Nancy;
using Nancy.Bootstrapper;
using Nancy.Testing;

namespace Drey.Server.Tests
{
    public abstract class NancyTestingBase
    {
        INancyBootstrapper _bootstrapper;
        Browser _browser;

        public Browser TestBrowser { get { return _browser; } }
        public INancyBootstrapper Bootstrapper { get { return _bootstrapper; } }

        public NancyTestingBase()
        {
            _bootstrapper = new TestNancyBootstrapper();
            _browser = new Browser(_bootstrapper, defaults: to => to.Accept("application/json"));
        }
    }
}