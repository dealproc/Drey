namespace Drey.Configuration.Repositories
{
    public interface IGlobalSettingsRepository
    {
        void SaveSetting(string key, string value);
        string GetSetting(string key);
    }
}