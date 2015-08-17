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
                cn.Execute("DELETE FROM GlobalSetting WHERE [Key] = @key", new { key = key });
                cn.Execute("INSERT INTO GlobalSetting ([Key], [Value], [CreatedOn], [UpdatedOn]) VALUES (@key, @value, @createdon, @updatedon)",
                    new { key = key, value = value, createdon = DateTime.Now, updatedon = DateTime.Now });
            });
        }

        public string GetSetting(string key)
        {
            return Execute((cn) => cn.ExecuteScalar("SELECT [Value] FROM GlobalSetting WHERE [Key] = @key", new { key = key }).ToString());
        }
    }
}
