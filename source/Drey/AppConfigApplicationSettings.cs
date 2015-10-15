using Drey.Nut;

using System;
using System.Configuration;
using System.Security.Permissions;

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

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}