using Drey.Nut;
using System;
using System.Configuration;

namespace Drey
{
    public class AppConfigApplicationSettings : MarshalByRefObject, IApplicationSettings
    {
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }
    }
}