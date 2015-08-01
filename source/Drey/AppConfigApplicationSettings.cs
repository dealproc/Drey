using Drey.Nut;
using System.Configuration;

namespace Drey
{
    public class AppConfigApplicationSettings : IApplicationSettings
    {
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }
    }
}