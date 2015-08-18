using Drey.Configuration.Repositories;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Configuration.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        readonly IEventBus _eventBus;
        readonly IGlobalSettingsRepository _globalSettingsRepository;

        public GlobalSettingsService(IEventBus eventBus, IGlobalSettingsRepository globalSettingsRepository)
        {
            _eventBus = eventBus;
            _globalSettingsRepository = globalSettingsRepository;
        }

        public bool StoreSettings(ViewModels.GlobalSettingsPmo settings)
        {
            UpdateServerHostname(settings.ServerHostname);
            UpdateSSLCertificate(settings.SSLPfx);

            _eventBus.Publish(new Infrastructure.Events.RecycleApp());

            return true;
        }

        public string GetServerHostname()
        {
            return _globalSettingsRepository.GetSetting("ServerHostname");
        }
        public X509Certificate2 GetCertificate()
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(_globalSettingsRepository.GetSetting("SSLPfx"));

                if (buffer == null || buffer.Length == 0) { return null; }

                return new X509Certificate2(buffer);
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public void UpdateServerHostname(string serverHostName)
        {
            if (string.IsNullOrWhiteSpace(serverHostName)) { throw new ArgumentNullException(serverHostName); }
            _globalSettingsRepository.SaveSetting("ServerHostname", serverHostName);
        }

        public void UpdateSSLCertificate(byte[] cert)
        {
            if (cert == null) { throw new ArgumentNullException("cert"); }
            _globalSettingsRepository.SaveSetting("SSLPfx", Convert.ToBase64String(cert));
        }

        public bool HasValidSettings()
        {
            bool hasHostname = !string.IsNullOrWhiteSpace(GetServerHostname());
            bool hasCertificate = GetCertificate() != null;
            return hasHostname && hasCertificate;
        }

        public HttpClient GetHttpClient()
        {
            var url = new Uri(GetServerHostname());
            var wrh = new WebRequestHandler();

            var cert = GetCertificate();
            if (cert != null)
            {
                wrh.ClientCertificates.Add(cert);
            }

            var result = new HttpClient(wrh) { BaseAddress = url };

            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return result;
        }
    }
}