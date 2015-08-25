using System;
namespace Drey.Nut
{
    [Serializable]
    public class DefaultAppSetting
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}