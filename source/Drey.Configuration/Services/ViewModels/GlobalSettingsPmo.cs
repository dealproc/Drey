using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class GlobalSettingsPmo
    {
        /// <summary>
        /// Gets or sets the server hostname.
        /// </summary>
        [Required]
        public string ServerHostname { get; set; }

        /// <summary>
        /// Gets or sets the SSL thumbprint if the server is setup with a self-signed certificate.
        /// <remarks>Leave this blank if using a CA issued certificate on the server.</remarks>
        /// </summary>
        public string SSLThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the SSL PFX.
        /// </summary>
        [Required]
        public byte[] SSLPfx { get; set; }
    }
}