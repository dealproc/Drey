using Microsoft.Owin.Testing;

using Nancy.Bootstrapper;
using Nancy.Testing;

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests
{
    [Collection("Package Management")]
    public abstract class NancyTestingBase : IDisposable
    {
        bool _disposed;
        INancyBootstrapper _bootstrapper;
        Browser _browser;

        public Browser TestBrowser { get { return _browser; } }
        public INancyBootstrapper Bootstrapper { get { return _bootstrapper; } }
        public string HostingUri { get { return "http://localhost:1/"; } }

        public NancyTestingBase()
        {
            _bootstrapper = new TestNancyBootstrapper();
            _browser = new Browser(_bootstrapper, defaults: to => to.Accept("application/json"));
        }
        ~NancyTestingBase()
        {
            Dispose(false);
        }

        protected Task WithOwinServer(Func<HttpClient, Task> test)
        {
            using (var server = TestServer.Create<TestingStartup>())
            {
                var client = server.HttpClient;

                client.DefaultRequestHeaders
                    .Add("Accept", "application/json");

                return test.Invoke(client);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            _disposed = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_bootstrapper != null)
            {
                _bootstrapper.Dispose();
                _bootstrapper = null;
            }

            if (!disposing || _disposed) { return; }
        }
    }
}