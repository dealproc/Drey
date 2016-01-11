using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class ServerHostnamePmo
    {
        /// <summary>
        /// Gets or sets the current hostname.
        /// </summary>
        public string CurrentHostname { get; set; }

        public string CurrentServerCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the new hostname.
        /// </summary>
        [Required]
        public string NewHostname { get; set; }

        /// <summary>
        /// Gets or sets the new server certificate thumbprint.
        /// </summary>
        /// <value>
        /// The new server certificate thumbprint.
        /// </value>
        public string NewServerCertificateThumbprint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHostnamePmo"/> class.
        /// </summary>
        public ServerHostnamePmo()
        {
            CurrentHostname = string.Empty;
            CurrentServerCertificateThumbprint = string.Empty;
            NewHostname = string.Empty;
            NewServerCertificateThumbprint = string.Empty;
        }
    }
}