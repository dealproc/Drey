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

        public string NewServerCertificateThumbprint { get; set; }

        public ServerHostnamePmo()
        {
            CurrentHostname = string.Empty;
            CurrentServerCertificateThumbprint = string.Empty;
            NewHostname = string.Empty;
            NewServerCertificateThumbprint = string.Empty;
        }
    }
}