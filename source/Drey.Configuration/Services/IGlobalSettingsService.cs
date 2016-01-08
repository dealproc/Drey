using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Configuration.Services
{
    public interface IGlobalSettingsService
    {
        /// <summary>
        /// Gets the certificate from the Global Settings store.
        /// </summary>
        /// <returns></returns>
        X509Certificate2 GetCertificate();

        /// <summary>
        /// Gets the server hostname from the Global Settings store.
        /// </summary>
        /// <returns></returns>
        ViewModels.ServerHostnamePmo GetServerHostname();
        
        /// <summary>
        /// Stores the initial settings in the Global Settings store.
        /// <remarks>This method should only be used for a new deployment.  Updates to settings should be done via the UpdateServerHostname/UpdateSSLCertificate method(s)</remarks>
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        bool StoreSettings(Drey.Configuration.Services.ViewModels.GlobalSettingsPmo settings);
        
        /// <summary>
        /// Updates the server hostname in the Global Settings store.
        /// </summary>
        /// <param name="serverHostName">Name of the server host.</param>
        void UpdateHostDetails(ViewModels.ServerHostnamePmo pmo);
        
        /// <summary>
        /// Updates the SSL certificate in the Global Settings store.
        /// </summary>
        /// <param name="cert">The cert.</param>
        void UpdateClientCertificate(byte[] cert);

        /// <summary>
        /// Determines whether the global settings are valid or not.
        /// </summary>
        /// <returns></returns>
        bool HasValidSettings();

        /// <summary>
        /// Manufacturers an Http Client with default values.
        /// </summary>
        /// <returns></returns>
        HttpClient GetHttpClient();
    }
}