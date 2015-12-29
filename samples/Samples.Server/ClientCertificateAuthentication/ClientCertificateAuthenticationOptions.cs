using Microsoft.Owin.Security;
using System.IdentityModel.Selectors;

namespace Samples.Server
{
    public class ClientCertificateAuthenticationOptions : AuthenticationOptions
    {
        public X509CertificateValidator Validator { get; set; }
        public bool CreateExtendedClaimSet { get; set; }

        public ClientCertificateAuthenticationOptions() : base("X.509")
        {
            Validator = X509CertificateValidator.ChainTrust;
            CreateExtendedClaimSet = false;
        }
    }
}
