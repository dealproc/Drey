using Nancy;
using Nancy.TinyIoc;

namespace Drey.Server.Tests
{
    public class TestNancyBootstrapper : DefaultNancyBootstrapper
    {
        public static string TEST_PACKAGE_DIR = @"c:\packages_test";
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var filesvc = new Drey.Server.Services.FilesytemFileService(TEST_PACKAGE_DIR);
            container.Register<Drey.Server.Services.IFileService>(filesvc);
            container.Register<Drey.Server.Services.IReleaseStore, Fixtures.ReleasesStorage>();
            container.Register<Drey.Server.Services.IPackageService, Drey.Server.Services.PackageService>();
        }
    }
}