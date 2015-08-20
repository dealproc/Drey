namespace Drey.Configuration.DataModel
{
    public class PackageConnectionString : DataModelBase
    {
        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the provider that this connection setting is for.
        /// </summary>
        public string ProviderName { get; set; }
    }
}