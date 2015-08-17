using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;
using System.IO;
using System.Linq;

namespace Drey.Configuration.Modules
{
    public class SetupModule : NancyModule
    {
        readonly Services.IGlobalSettingsService _globalSettingsService;

        public SetupModule(Services.IGlobalSettingsService globalSettingsService) : base("/Setup")
        {
            _globalSettingsService = globalSettingsService;

            Get["/"] = GetIndex;
            Post["/"] = CommitSettings;
        }

        private dynamic GetIndex(dynamic arg)
        {
            return View["index", new Services.ViewModels.GlobalSettingsPmo()];
        }

        private dynamic CommitSettings(dynamic arg)
        {
            var settingsPmo = this.BindAndValidate<Services.ViewModels.GlobalSettingsPmo>();

            if (Request.Files.Any())
            {
                using (var ms = new MemoryStream())
                {
                    Request.Files.First().Value.CopyTo(ms);
                    settingsPmo.SSLPfx = ms.ToArray();
                }
            }

            ModelValidationResult.Errors.Clear();
            this.Validate(settingsPmo);

            if (ModelValidationResult.IsValid)
            {
                _globalSettingsService.StoreSettings(settingsPmo);
                return Response.AsRedirect("~/", Nancy.Responses.RedirectResponse.RedirectType.Permanent);
            }

            return View["index", settingsPmo];
        }
    }
}