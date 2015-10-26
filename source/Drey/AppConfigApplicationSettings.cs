using Drey.Logging;
using Drey.Nut;

using System;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;

namespace Drey
{
    public class AppConfigApplicationSettings : MarshalByRefObject, IApplicationSettings, IGlobalSettings
    {
        static ILog _log = LogProvider.For<AppConfigApplicationSettings>();

        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }

        public bool Exists(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        public void Register(string key, string value = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {key} | {value}", key, value);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}