using Drey.Logging;

using System;
using System.Net;

namespace Drey.CertificateValidation
{
    /// <summary>
    /// Utilize this when the broker has a SSL certificate issued from a registered Authority (Comodo; etc.)
    /// </summary>
    [Serializable]
    public class AuthorityIssuedServerCertificateValidation : ICertificateValidation
    {
        static ILog _log = LogProvider.For<AuthorityIssuedServerCertificateValidation>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            _log.Info("Trusting server certificates from a registered authority.");
            ServicePointManager.ServerCertificateValidationCallback = null;
        }
    }
}