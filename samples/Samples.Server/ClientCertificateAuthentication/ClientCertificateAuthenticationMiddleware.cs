using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace Samples.Server
{
    public class ClientCertificateAuthenticationMiddleware : AuthenticationMiddleware<ClientCertificateAuthenticationOptions>
    {
        public ClientCertificateAuthenticationMiddleware(OwinMiddleware next, ClientCertificateAuthenticationOptions options) : base(next, options) { }

        protected override AuthenticationHandler<ClientCertificateAuthenticationOptions> CreateHandler()
        {
            return new ClientCertificateAuthenticationHandler();
        }
    }
}