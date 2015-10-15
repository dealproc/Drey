using FakeItEasy;
using Microsoft.AspNet.SignalR;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drey.Server.Tests
{
    public class TestNancyBootstrapper : DefaultNancyBootstrapper
    {
        public static string TEST_PACKAGE_DIR = @"c:\packages_test";
        ApiTestFixture _testFixture;

        public TestNancyBootstrapper(ApiTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var filesvc = new Drey.Server.Services.FilesytemFileService(TEST_PACKAGE_DIR);

            container.Register<Drey.Server.Services.IFileService>(filesvc);
            container.Register<Drey.Server.Services.IReleaseStore, Fixtures.ReleasesStorage>();
            container.Register<Drey.Server.Services.IPackageService, Drey.Server.Services.PackageService>();

            container.Register<Directors.IListLogsDirector>(_testFixture.ListLogsDirector);
            container.Register<Directors.IOpenLogFileDirector>(_testFixture.OpenLogFileDirector);
        }
    }
}