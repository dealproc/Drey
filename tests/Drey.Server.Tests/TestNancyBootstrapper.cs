using Nancy;
using Nancy.TinyIoc;

namespace Drey.Server.Tests
{
    public class TestNancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var filesvc = new Drey.Server.Services.FilesytemFileService(@"c:\packages_test");
            container.Register<Drey.Server.Services.IFileService>(filesvc);
            container.Register<Drey.Server.Services.IPackageStore, Fixtures.PackageStore>();
            container.Register<Drey.Server.Services.IPackageService, Drey.Server.Services.PackageService>();
            //base.ConfigureApplicationContainer(container);
        }
    }
}