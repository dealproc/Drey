namespace Drey.Configuration.DataModel
{
    /// <summary>
    /// Runtime-wide settings all packages can use.
    /// </summary>
    public class GlobalSetting : DataModelBase
    {
        /// <summary>
        /// The setting's reference key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The setting's value.
        /// </summary>
        public string Value { get; set; }
    }
}