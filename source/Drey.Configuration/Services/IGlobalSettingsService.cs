using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Configuration.Services
{
    public interface IGlobalSettingsService
    {
        X509Certificate2 GetCertificate();
        string GetServerHostname();
        bool StoreSettings(Drey.Configuration.Services.ViewModels.GlobalSettingsPmo settings);
        void UpdateServerHostname(string serverHostName);
        void UpdateSSLCertificate(byte[] cert);
        bool HasValidSettings();
        HttpClient GetHttpClient();
    }
}