using Drey.Configuration.Services;

using Nancy;
using Nancy.ModelBinding;

using System;

namespace Drey.Configuration.Modules
{
    public class AppletModule : BaseModule
    {
        IPackageService _packageService;

        public AppletModule(IEventBus eventBus, IGlobalSettingsService globalSettingsService, IPackageService packageService) : base(eventBus, globalSettingsService, "/applet/{id}", true)
        {
            _packageService = packageService;

            Get["/"] = Dashboard;

            Get["/appSetting/new"] = AddAppSetting;
            Get["/appSetting/{key}/edit"] = EditAppSetting;
            Post["/appSetting"] = SaveAppSetting;

            Get["/connectionStrings/new"] = AddConnectionString;
            Get["/connectionStrings/{name}/edit"] = EditConnectionString;
            Post["/connectionStrings"] = SaveConnectionString;
        }

        private dynamic Dashboard(dynamic arg)
        {
            return Negotiate.WithView("index").WithModel(_packageService.Dashboard((string)arg.id));
        }

        private dynamic AddAppSetting(dynamic arg)
        {
            var model = new Services.ViewModels.AppSettingPmo { PackageId = (string)arg.id };
            return Negotiate.WithView("appSettingEditor").WithModel(model);
        }

        private dynamic EditAppSetting(dynamic arg)
        {
            var model = _packageService.GetAppSetting((string)arg.id, (string)arg.key);
            
            if (model == null)
            {
                return HttpStatusCode.NotFound;
            }

            return Negotiate.WithView("appSettingEditor").WithModel(model);
        }

        private dynamic SaveAppSetting(dynamic arg)
        {
            var model = this.BindAndValidate<Services.ViewModels.AppSettingPmo>();

            if (!ModelValidationResult.IsValid)
            {
                return Negotiate.WithView("appSettingEditor").WithModel(model);
            }

            _packageService.RecordAppSetting(model);
            return Response.AsRedirect("~/applet/" + model.PackageId);
        }


        private dynamic AddConnectionString(dynamic arg)
        {
            return Negotiate.WithView("connStringEditor").WithModel(new Services.ViewModels.ConnectionStringPmo { PackageId = (string)arg.id, Providers = _packageService.ConnectionFactoryProviders() });
        }

        private dynamic EditConnectionString(dynamic arg)
        {
            var model = _packageService.GetConnectionString((string)arg.id, (string)arg.name);
            
            if (model == null)
            {
                return HttpStatusCode.NotFound;
            }

            return Negotiate.WithView("connStringEditor").WithModel(model);
        }

        private dynamic SaveConnectionString(dynamic arg)
        {
            var model = this.BindAndValidate<Services.ViewModels.ConnectionStringPmo>();

            if (!ModelValidationResult.IsValid)
            {
                model.Providers = _packageService.ConnectionFactoryProviders();
                return Negotiate.WithView("connStringEditor").WithModel(model);
            }

            _packageService.RecordConnectionString(model);
            return Response.AsRedirect("~/applet/" + model.PackageId);
        }
    }
}