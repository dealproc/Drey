using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Samples.Server
{
    public class ClientCertificateAuthenticationHandler : AuthenticationHandler<ClientCertificateAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");

            if (cert == null) { return Task.FromResult<AuthenticationTicket>(null); }

            try
            {
                Options.Validator.Validate(cert);
            }
            catch (SecurityTokenValidationException)
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }

            var identity = new ClaimsIdentity(Options.AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(GetClaimsFromCertificate(cert, cert.Issuer, Options.CreateExtendedClaimSet));

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            return Task.FromResult<AuthenticationTicket>(ticket);
        }

        private IEnumerable<Claim> GetClaimsFromCertificate(X509Certificate2 cert, string p1, bool p2)
        {
            yield return new Claim(ClaimTypes.Thumbprint, cert.Thumbprint);
            yield return new Claim(ClaimTypes.Name, cert.Thumbprint);
            yield return new Claim(ClaimTypes.NameIdentifier, cert.Thumbprint);
            yield return new Claim(Drey.Server.ServerConstants.ClaimTypes.Scope, Drey.Server.ServerConstants.Scopes.Admin);
        }
    }
}