using Nancy;

namespace Drey.Configuration.Modules
{
    public class HomeModule : BaseModule
    {
        public HomeModule(Services.IGlobalSettingsService globalSettingsService) : base(globalSettingsService)
        {
            Get["/"] = _ => Negotiate.WithView("index").WithModel(new { });
        }
    }
}