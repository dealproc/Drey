using Drey.Configuration.Repositories;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Configuration.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        readonly IGlobalSettingsRepository _globalSettingsRepository;

        public GlobalSettingsService(IGlobalSettingsRepository globalSettingsRepository)
        {
            _globalSettingsRepository = globalSettingsRepository;
        }

        public bool StoreSettings(ViewModels.GlobalSettingsPmo settings)
        {
            UpdateServerHostname(settings.ServerHostname);
            UpdateSSLCertificate(settings.SSLPfx);
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
    }
}