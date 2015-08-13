using Nancy;
using Nancy.TinyIoc;

namespace Samples.Server
{
    public class SampleServerBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var filesvc = new Drey.Server.Services.FilesytemFileService(@"c:\packages_test");
            container.Register<Drey.Server.Services.IFileService>(filesvc);
            container.Register<Drey.Server.Services.IPackageStore, Samples.Server.Services.InMemory.PackageStore>();
            container.Register<Drey.Server.Services.IPackageService, Drey.Server.Services.PackageService>();
            //base.ConfigureApplicationContainer(container);
        }
    }
}