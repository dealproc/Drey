using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.Services.ViewModels
{
    public class ConnectionStringPmo
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
        /// Gets or sets the name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the providers.
        /// </summary>
        public IDictionary<string, string> Providers { get; set; }
    }
}