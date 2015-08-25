using Drey.Configuration.Services;
using Nancy;

namespace Drey.Configuration.Modules
{
    public class HomeModule : BaseModule
    {
        public HomeModule(Services.IGlobalSettingsService globalSettingsService, IPackageService packageService) : base(globalSettingsService)
        {
            Get["/"] = _ => Negotiate.WithView("index").WithModel(packageService.LatestRegisteredReleases());
        }
    }
}