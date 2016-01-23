using FakeItEasy;

using Microsoft.Owin.Testing;

using Nancy.Testing;

using System;
using System.Collections.Generic;
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

        public Server.Directors.IListLogsDirector ListLogsDirector { get; private set; }
        public Server.Directors.IOpenLogFileDirector OpenLogFileDirector { get; private set; }
        public Server.Directors.IRecycleClientDirector RecycleClientDirector { get; private set; }
        public Server.Services.INugetApiClaimsValidator NugetApiClaimsValidator { get; private set; }

        public ApiTestFixture()
        {
            ListLogsDirector = A.Fake<Server.Directors.IListLogsDirector>();
            A.CallTo(() => ListLogsDirector.PendingTask).Returns(DomainModel.Response<IEnumerable<string>>.Success(string.Empty, (new string[] { "one", "two" }).AsEnumerable()));
            A.CallTo(() => ListLogsDirector.Initiate(A<string>.Ignored, A<DomainModel.Request<DomainModel.Empty>>.Ignored)).DoesNothing();

            OpenLogFileDirector = A.Fake<Server.Directors.IOpenLogFileDirector>();
            A.CallTo(() => OpenLogFileDirector.PendingTask).Returns(DomainModel.Response<byte[]>.Success(string.Empty, new byte[10]));

            RecycleClientDirector = A.Fake<Server.Directors.IRecycleClientDirector>();
            A.CallTo(() => RecycleClientDirector.PendingTask).Returns(DomainModel.Response<DomainModel.Empty>.Success(string.Empty, new DomainModel.Empty()));

            NugetApiClaimsValidator = A.Fake<Server.Services.INugetApiClaimsValidator>();
            A.CallTo(() => NugetApiClaimsValidator.Validate(A<System.Security.Claims.Claim[]>.Ignored)).Returns(true);

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

            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            if (_bootstrapper != null)
            {
                _bootstrapper.Dispose();
                _bootstrapper = null;
            }

            _disposed = true;
        }
    }
}