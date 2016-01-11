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
    /// <summary>
    /// Setup UI.
    /// </summary>
    public class SetupModule : BaseModule
    {
        static readonly ILog _log = LogProvider.For<SetupModule>();

        public SetupModule(IEventBus eventBus, Services.IGlobalSettingsService globalSettingsService)
            : base(eventBus, globalSettingsService, "/Setup", false)
        {
            Get["/"] = GetIndex;
            Post["/"] = CommitSettings;

            Get["/ServerUrl"] = UpdateServerUrl;
            Post["/ServerUrl"] = SaveNewServerUrl;

            Get["/ClientCertificate"] = UpdateClientCertificate;
            Post["/ClientCertificate"] = SaveNewClientCertificate;
        }

        /// <summary>
        /// Gets a view allowing the user to do an initial system setup by providing the following items:
        ///  * Client Certificate
        ///  * Server Hostname
        ///  * Server SSL Thumbprint (for self-signed server certs).
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        private dynamic GetIndex(dynamic arg)
        {
            _log.Debug("Index has been accessed.");
            return View["index", new Services.ViewModels.GlobalSettingsPmo()];
        }

        /// <summary>
        /// Commits the initial settings set by the user to the global settings store.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Presents a view where the user may provide a new client certificate for use by the runtime environment.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        private dynamic UpdateClientCertificate(dynamic arg)
        {
            _log.Debug("Attempting to update Client Certificate.");
            return Negotiate.WithView("ClientCertificate");
        }

        // TODO: Restructure this to re-boot all loaded applets.        
        /// <summary>
        /// Saves the new client certificate to the global settings store.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
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
                GlobalSettingsService.UpdateClientCertificate(cert);

                return RestartAppDomains();
            }
            catch (Exception ex)
            {
                ModelValidationResult.Errors.Add(string.Empty, ex.Message);
            }

            return Negotiate.WithView("ClientCertificate");
        }

        /// <summary>
        /// Renders a view with the server url and client certificate information presently configured.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        private dynamic UpdateServerUrl(dynamic arg)
        {
            _log.Debug("Attempting to update Server Url.");
            return Negotiate.WithView("ServerUrl").WithModel(GlobalSettingsService.GetServerHostname());
        }

        // TODO: Restructure this to re-boot all loaded applets.        
        /// <summary>
        /// Saves the updated server url information to the global settings store.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        private dynamic SaveNewServerUrl(dynamic arg)
        {
            _log.Debug("Updating Server Url.");
            var model = this.BindAndValidate<Services.ViewModels.ServerHostnamePmo>();

            if (ModelValidationResult.IsValid)
            {
                GlobalSettingsService.UpdateHostDetails(model);
                return RestartAppDomains();
            }

            var currentHostInfo = GlobalSettingsService.GetServerHostname();
            model.CurrentHostname = currentHostInfo.CurrentHostname;
            model.CurrentServerCertificateThumbprint = currentHostInfo.CurrentServerCertificateThumbprint;
            return Negotiate.WithView("ServerUrl").WithModel(model);
        }
    }
}