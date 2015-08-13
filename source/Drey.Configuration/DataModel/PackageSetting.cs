namespace Drey.Configuration.DataModel
{
    public class PackageSetting : DataModelBase
    {
        /// <summary>
        /// Gets or sets the package this setting is associated with.
        /// </summary>
        public RegisteredPackage Package { get; set; }
        
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