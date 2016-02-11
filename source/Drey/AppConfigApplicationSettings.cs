using Drey.Logging;
using Drey.Nut;

using System;
using System.Configuration;

namespace Drey
{
    /// <summary>
    /// Allows access to the app.config application settings
    /// </summary>
    public class AppConfigApplicationSettings : MarshalByRefObject, IApplicationSettings, IGlobalSettings
    {
        static ILog _log = LogProvider.For<AppConfigApplicationSettings>();

        /// <summary>
        /// Gets the application setting associated with the key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }

        /// <summary>
        /// Check to see if a given key exists within the application settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        /// <summary>
        /// Registers an application key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Register(string key, string value = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {key} | {value}", key, value);
        }
    }
}