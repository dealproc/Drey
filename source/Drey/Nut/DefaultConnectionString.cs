using System;
namespace Drey.Nut
{
    [Serializable]
    public class DefaultConnectionString
    {
        /// <summary>
        /// Gets or sets the connectionString name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Gets or sets the provider factory's name.
        /// </summary>
        public string ProviderName { get; set; }
    }
}