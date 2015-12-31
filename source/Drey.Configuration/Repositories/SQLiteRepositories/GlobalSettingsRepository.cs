using Dapper;

using Drey.Nut;

using System;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class GlobalSettingsRepository : SqlRepository, IGlobalSettingsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSettingsRepository"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public GlobalSettingsRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        public GlobalSettingsRepository(string databaseNameAndPath) : base(databaseNameAndPath) { }

        /// <summary>
        /// Saves the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SaveSetting(string key, string value)
        {
            ExecuteWithTransaction((cn) =>
            {
                cn.Execute("DELETE FROM GlobalSettings WHERE [Key] = @key", new { key = key });
                cn.Execute("INSERT INTO GlobalSettings ([Key], [Value], [CreatedOn], [UpdatedOn]) VALUES (@key, @value, @createdon, @updatedon)",
                    new { key = key, value = value, createdon = DateTime.Now, updatedon = DateTime.Now });
            });
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return GetSetting(key); }
        }

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetSetting(string key)
        {
            return Execute((cn) =>
            {
                try
                {
                    return cn.ExecuteScalar("SELECT [Value] FROM GlobalSettings WHERE [Key] = @key", new { key = key }).ToString();
                }
                catch (NullReferenceException)
                {
                    return string.Empty;
                }
            });
        }
    }
}