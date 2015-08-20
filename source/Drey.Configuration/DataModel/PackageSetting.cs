namespace Drey.Configuration.DataModel
{
    public class PackageSetting : DataModelBase
    {
        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the lookup key for this setting.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets this setting's value.
        /// </summary>
        public string Value { get; set; }
    }
}