namespace Drey.Configuration.Repositories
{
    public interface IGlobalSettingsRepository
    {
        /// <summary>
        /// Saves the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SaveSetting(string key, string value);

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetSetting(string key);
    }
}