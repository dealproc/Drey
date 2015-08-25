using System.ComponentModel.DataAnnotations;
namespace Drey.Configuration.Services.ViewModels
{
    public class AppSettingPmo
    {
        public int Id { get; set; }

        [Required]
        public string PackageId { get; set; }
        
        [Required]
        public string Key { get; set; }
        
        [Required]
        public string Value { get; set; }
    }
}