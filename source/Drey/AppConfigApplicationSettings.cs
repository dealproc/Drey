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

        public void Register(System.Collections.Generic.IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}