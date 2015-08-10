using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Drey.Client.Utilities
{
    public static class HttpClientFactory
    {
        public static HttpClient Create()
        {
            var url = ConfigurationManager.AppSettings[ClientConstants.BrokerUrl];
            var cert = new X509Certificate2(ConfigurationManager.AppSettings[ClientConstants.CertificatePath]);
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(cert);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url)
            };

            client.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}