using Drey.Configuration.Services;
using Drey.Nut;
using Nancy;
using Nancy.Extensions;

namespace Drey.Configuration.Modules
{
    public class BaseModule : NancyModule
    {
        readonly IGlobalSettingsService _globalSettingsService;

        public BaseModule(Services.IGlobalSettingsService globalSettingsService) : base() 
        {
            _globalSettingsService = globalSettingsService;
            WireSetupPipeline();
        }

        public BaseModule(Services.IGlobalSettingsService globalSettingsService, string modulePath) : base(modulePath) 
        {
            _globalSettingsService = globalSettingsService;
            WireSetupPipeline();
        }

        private void WireSetupPipeline()
        {
            this.Before.AddItemToEndOfPipeline(context =>
            {
                var isNotSetupPath = !context.Request.Path.Equals("/Setup") ;
                var globalSettingsAreNotValid = !_globalSettingsService.HasValidSettings();
                if (isNotSetupPath && globalSettingsAreNotValid)
                {
                    return context.GetRedirect("~/Setup");
                }

                return (Response)null;
            });
        }
    }
}