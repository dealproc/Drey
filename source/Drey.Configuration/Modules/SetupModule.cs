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
    public class SetupModule : NancyModule
    {
        static readonly ILog _log = LogProvider.For<SetupModule>();

        readonly Services.IGlobalSettingsService _globalSettingsService;

        public SetupModule(Services.IGlobalSettingsService globalSettingsService)
            : base("/Setup")
        {
            _globalSettingsService = globalSettingsService;

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
                _globalSettingsService.StoreSettings(settingsPmo);
                return Response.AsRedirect("~/", Nancy.Responses.RedirectResponse.RedirectType.Permanent);
            }

            return View["index", settingsPmo];
        }

        private dynamic UpdateClientCertificate(dynamic arg)
        {
            _log.Debug("Attempting to update Client Certificate.");
            return Negotiate.WithView("ClientCertificate");
        }

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
                _globalSettingsService.UpdateSSLCertificate(cert);

                return Response.AsRedirect("~/");
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
            return Negotiate.WithView("ServerUrl").WithModel(new Services.ViewModels.ServerHostnamePmo { CurrentHostname = _globalSettingsService.GetServerHostname() });
        }

        private dynamic SaveNewServerUrl(dynamic arg)
        {
            _log.Debug("Updating Server Url.");
            var model = this.BindAndValidate<Services.ViewModels.ServerHostnamePmo>();

            if (ModelValidationResult.IsValid)
            {
                _globalSettingsService.UpdateServerHostname(model.NewHostname);
                return Response.AsRedirect("~/");
            }

            model.CurrentHostname = _globalSettingsService.GetServerHostname();
            return Negotiate.WithView("ServerUrl").WithModel(model);
        }
    }
}