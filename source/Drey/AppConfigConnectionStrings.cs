using Drey.Logging;
using Drey.Nut;

using System;
using System.Configuration;

namespace Drey
{
    /// <summary>
    /// Allows access to the connection strings element in the app.config of the runtime.
    /// </summary>
    public class AppConfigConnectionStrings : MarshalByRefObject, IConnectionStrings
    {
        static readonly ILog _log = LogProvider.For<AppConfigConnectionStrings>();

        /// <summary>
        /// Gets the connection string for the given key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return ConfigurationManager.ConnectionStrings[key].ConnectionString; }
        }

        /// <summary>
        /// Checks to see if a specific key exists in the underlying store.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            return ConfigurationManager.ConnectionStrings["name"] != null;
        }

        /// <summary>
        /// Registers a connection string, with a given "name" and provider name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        public void Register(string name, string connectionString, string providerName = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {name} | {connectionString} | {providerName}", name, connectionString, providerName);
        }
    }
}