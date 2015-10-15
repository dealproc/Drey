using FakeItEasy;

using Microsoft.Owin.Testing;

using Nancy.Testing;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Drey.Server.Tests
{
    public class ApiTestFixture : IDisposable
    {
        bool _disposed;
        TestNancyBootstrapper _bootstrapper;
        Browser _browser;

        public Browser TestBrowser { get { return _browser; } }
        public TestNancyBootstrapper Bootstrapper { get { return _bootstrapper; } }
        public string HostingUri { get { return "http://localhost:1/"; } }

        public Directors.IListLogsDirector ListLogsDirector { get; private set; }
        public Directors.IOpenLogFileDirector OpenLogFileDirector { get; private set; }

        public ApiTestFixture()
        {
            ListLogsDirector = A.Fake<Directors.IListLogsDirector>(opts =>
            {
            });
            A.CallTo(() => ListLogsDirector.PendingTask).Returns((new string[] { "one", "two" }).AsEnumerable());
            A.CallTo(() => ListLogsDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.Empty>>.Ignored)).DoesNothing();

            OpenLogFileDirector = A.Fake<Directors.IOpenLogFileDirector>();
            A.CallTo(() => OpenLogFileDirector.PendingTask).Returns(new byte[10]);

            _bootstrapper = new TestNancyBootstrapper(this);
            _browser = new Browser(_bootstrapper, defaults: to => to.Accept("application/json"));

            TestingStartup.BootstrapperFunc = () => Bootstrapper;
        }

        ~ApiTestFixture()
        {
            Dispose(false);
        }

        public Task WithOwinServer(Func<HttpClient, Task> test)
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