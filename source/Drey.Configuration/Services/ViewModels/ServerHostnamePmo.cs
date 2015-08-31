using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class ServerHostnamePmo
    {
        /// <summary>
        /// Gets or sets the current hostname.
        /// </summary>
        public string CurrentHostname { get; set; }

        /// <summary>
        /// Gets or sets the new hostname.
        /// </summary>
        [Required]
        public string NewHostname { get; set; }
    }
}