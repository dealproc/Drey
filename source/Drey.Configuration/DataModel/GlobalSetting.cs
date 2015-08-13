namespace Drey.Configuration.DataModel
{
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