using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class AppSettingPmo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        [Required]
        public string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}