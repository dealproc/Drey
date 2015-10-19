using Drey.Configuration.Services;
using Drey.Logging;

using Nancy;

namespace Drey.Configuration.Modules
{
    public class HomeModule : BaseModule
    {
        static ILog _log = LogProvider.For<HomeModule>();


        public HomeModule(Services.IGlobalSettingsService globalSettingsService, IPackageService packageService) : base(globalSettingsService)
        {
            Get["/"] = _ =>
            {
                _log.Debug("Home page being accessed.");
                return Negotiate.WithView("index").WithModel(packageService.LatestRegisteredReleases());
            };
        }
    }
}