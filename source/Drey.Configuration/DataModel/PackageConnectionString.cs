namespace Drey.Configuration.DataModel
{
    public class PackageConnectionString : DataModelBase
    {
        /// <summary>
        /// Gets or sets the package this connection string is associated with.
        /// </summary>
        public RegisteredPackage Package { get; set; }
        
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