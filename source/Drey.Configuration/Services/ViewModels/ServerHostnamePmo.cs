using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class ServerHostnamePmo
    {
        public string CurrentHostname { get; set; }
        
        [Required]
        public string NewHostname { get; set; }
    }
}