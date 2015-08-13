using Drey.Nut;
using System;
using System.Configuration;

namespace Drey
{
    public class AppConfigApplicationSettings : MarshalByRefObject, IApplicationSettings, IGlobalSettings
    {
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }
    }
}