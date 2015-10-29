using Drey.Logging;

using Nancy;
using Nancy.ModelBinding;
using Nancy.Validation;

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Configuration.Modules
{
    public class SetupModule : BaseModule
    {
        static readonly ILog _log = LogProvider.For<SetupModule>();

        public SetupModule(IEventBus eventBus, Services.IGlobalSettingsService globalSettingsService) : base(eventBus, globalSettingsService, "/Setup", false)
        {
            Get["/"] = GetIndex;
            Post["/"] = CommitSettings;

            Get["/ServerUrl"] = UpdateServerUrl;
            Post["/ServerUrl"] = SaveNewServerUrl;

            Get["/ClientCertificate"] = UpdateClientCertificate;
            Post["/ClientCertificate"] = SaveNewClientCertificate;
        }

        private dynamic GetIndex(dynamic arg)
        {
            _log.Debug("Index has been accessed.");
            return View["index", new Services.ViewModels.GlobalSettingsPmo()];
        }

        private dynamic CommitSettings(dynamic arg)
        {
            _log.Debug("Committing Settings.");
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
                GlobalSettingsService.StoreSettings(settingsPmo);

                return RestartAppDomains();
            }

            return View["index", settingsPmo];
        }

        private dynamic UpdateClientCertificate(dynamic arg)
        {
            _log.Debug("Attempting to update Client Certificate.");
            return Negotiate.WithView("ClientCertificate");
        }

        // TODO: Restructure this to re-boot all loaded applets.
        private dynamic SaveNewClientCertificate(dynamic arg)
        {
            _log.Debug("Updating new Client Certificate.");
            if (!Request.Files.Any())
            {
                ModelValidationResult.Errors.Add(string.Empty, "File did not upload.");
            }

            var certFile = Request.Files.First();
            try
            {
                byte[] cert;
                using (var ms = new MemoryStream())
                {
                    certFile.Value.CopyTo(ms);
                    cert = ms.ToArray();
                }

                var clientSslObj = new X509Certificate2(cert, (string)null);
                GlobalSettingsService.UpdateSSLCertificate(cert);

                return RestartAppDomains();
            }
            catch (Exception ex)
            {
                ModelValidationResult.Errors.Add(string.Empty, ex.Message);
            }

            return Negotiate.WithView("ClientCertificate");
        }

        private dynamic UpdateServerUrl(dynamic arg)
        {
            _log.Debug("Attempting to update Server Url.");
            return Negotiate.WithView("ServerUrl").WithModel(new Services.ViewModels.ServerHostnamePmo { CurrentHostname = GlobalSettingsService.GetServerHostname() });
        }

        // TODO: Restructure this to re-boot all loaded applets.
        private dynamic SaveNewServerUrl(dynamic arg)
        {
            _log.Debug("Updating Server Url.");
            var model = this.BindAndValidate<Services.ViewModels.ServerHostnamePmo>();

            if (ModelValidationResult.IsValid)
            {
                GlobalSettingsService.UpdateServerHostname(model.NewHostname);
                return RestartAppDomains();
            }

            model.CurrentHostname = GlobalSettingsService.GetServerHostname();
            return Negotiate.WithView("ServerUrl").WithModel(model);
        }
    }
}