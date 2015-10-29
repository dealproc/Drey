using Drey.Configuration.Services;
using Drey.Logging;

using Nancy;
using Nancy.Security;

namespace Drey.Configuration.Modules
{
    public class HomeModule : BaseModule
    {
        static ILog _log = LogProvider.For<HomeModule>();

        public HomeModule(Services.IGlobalSettingsService globalSettingsService, IEventBus eventBus, IPackageService packageService) : base(eventBus, globalSettingsService, true)
        {
            Get["/"] = _ =>
            {
                _log.Debug("Home page being accessed.");
                return Negotiate.WithView("index").WithModel(packageService.LatestRegisteredReleases());
            };
            Get["/recycle"] = _ =>
            {
                _log.Debug("Attempting to recycle application.");
                return Negotiate.WithView("recycleapp").WithModel(packageService.LatestRegisteredReleases());
            };
            Post["/recycle"] = _ =>
            {
                this.ValidateCsrfToken();

                return RestartAppDomains();
            };
            Get["/pending"] = _ =>
            {
                return View["recyclepending"];
            };
        }
    }
}