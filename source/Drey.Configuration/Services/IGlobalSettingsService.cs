using System;
namespace Drey.Configuration.Services
{
    public interface IGlobalSettingsService
    {
        System.Security.Cryptography.X509Certificates.X509Certificate2 GetCertificate();
        string GetServerHostname();
        bool StoreSettings(Drey.Configuration.Services.ViewModels.GlobalSettingsPmo settings);
        void UpdateServerHostname(string serverHostName);
        void UpdateSSLCertificate(byte[] cert);
        bool HasValidSettings();
    }
}