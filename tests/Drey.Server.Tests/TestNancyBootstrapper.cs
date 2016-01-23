using Nancy;
using Nancy.TinyIoc;

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

            container.Register<Server.Services.IFileService>(filesvc);
            container.Register<Server.Services.IReleaseStore, Fixtures.ReleasesStorage>();
            container.Register<Server.Services.IPackageService, Drey.Server.Services.PackageService>();

            container.Register(_testFixture.ListLogsDirector);
            container.Register(_testFixture.OpenLogFileDirector);
            container.Register(_testFixture.RecycleClientDirector);
            container.Register(_testFixture.NugetApiClaimsValidator);
        }
    }
}