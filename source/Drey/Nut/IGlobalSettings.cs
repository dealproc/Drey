namespace Drey.Nut
{
    public interface IGlobalSettings
    {
        /// <summary>
        /// Gets the global setting associated with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The setting.</returns>
        string this[string key] { get; }
    }
}