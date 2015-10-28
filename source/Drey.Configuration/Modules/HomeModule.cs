using Drey.Configuration.Services;
using Drey.Logging;

using Nancy;
using Nancy.Security;

using System;
using System.Threading.Tasks;

namespace Drey.Configuration.Modules
{
    public class HomeModule : BaseModule
    {
        static ILog _log = LogProvider.For<HomeModule>();

        readonly IEventBus _eventBus;

        public HomeModule(Services.IGlobalSettingsService globalSettingsService, IEventBus eventBus, IPackageService packageService) : base(globalSettingsService)
        {
            _eventBus = eventBus;

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
                Task.Factory.StartNew(() =>
                {
                    _log.Debug("Waiting one second");
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    _log.Debug("Issuing recycle app command.");
                    _eventBus.Publish(new Infrastructure.Events.RecycleApp());
                });
                return Response.AsRedirect("~/pending", Nancy.Responses.RedirectResponse.RedirectType.SeeOther);
            };
            Get["/pending"] = _ =>
            {
                return View["recyclepending"];
            };
        }
    }
}