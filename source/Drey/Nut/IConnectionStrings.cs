using System.Collections.Generic;
namespace Drey.Nut
{
    public interface IConnectionStrings
    {
        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string this[string key] { get; }

        /// <summary>
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool Exists(string name);

        /// <summary>
        /// Registers a connection string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        void Register(string name, string connectionString, string providerName = "");
    }
}