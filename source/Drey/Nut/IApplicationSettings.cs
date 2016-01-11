using System.Collections.Generic;
namespace Drey.Nut
{
    /// <summary>
    /// Provides access to an applicaton settings store, scoped to a package.
    /// </summary>
    public interface IApplicationSettings
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
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool Exists(string key);
        
        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Register(string key, string value = "");
    }
}