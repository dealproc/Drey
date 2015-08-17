using Drey.Nut;
using System;
using Dapper;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class GlobalSettingsRepository : SqlRepository, IGlobalSettingsRepository
    {
        public GlobalSettingsRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        public void SaveSetting(string key, string value)
        {
            ExecuteWithTransaction((cn) =>
            {
                cn.Execute("DELETE FROM GlobalSettings WHERE [Key] = @key", new { key = key });
                cn.Execute("INSERT INTO GlobalSettings ([Key], [Value], [CreatedOn], [UpdatedOn]) VALUES (@key, @value, @createdon, @updatedon)",
                    new { key = key, value = value, createdon = DateTime.Now, updatedon = DateTime.Now });
            });
        }

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
