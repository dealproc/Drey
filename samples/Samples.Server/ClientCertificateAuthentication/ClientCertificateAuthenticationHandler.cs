using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Samples.Server
{
    public class ClientCertificateAuthenticationHandler : AuthenticationHandler<ClientCertificateAuthenticationOptions>
    {
        protected override async Task<Microsoft.Owin.Security.AuthenticationTicket> AuthenticateCoreAsync()
        {
            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");

            if (cert == null) { return null; }

            try
            {
                Options.Validator.Validate(cert);
            }
            catch
            {
                return null;
            }

            var claims = await GetClaimsFromCertificate(cert, cert.Issuer, Options.CreateExtendedClaimSet);

            var identity = new ClaimsIdentity(Options.AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(claims);

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            return ticket;
        }

        private Task<IEnumerable<Claim>> GetClaimsFromCertificate(X509Certificate2 cert, string p1, bool p2)
        {
            return Task.FromResult(new Claim[] {
                new Claim(ClaimTypes.Thumbprint, cert.Thumbprint),
                new Claim(ClaimTypes.Name, cert.SubjectName.Name),
                new Claim(ClaimTypes.NameIdentifier, cert.Thumbprint),
                new Claim(Drey.Server.ServerConstants.ClaimTypes.Scope, Drey.Server.ServerConstants.Scopes.Admin)
            }.AsEnumerable());
        }
    }
}