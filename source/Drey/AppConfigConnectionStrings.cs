using Drey.Logging;
using Drey.Nut;
using System;
using System.Configuration;
using System.Security.Permissions;

namespace Drey
{
    public class AppConfigConnectionStrings : MarshalByRefObject, IConnectionStrings
    {
        static readonly ILog _log = LogProvider.For<AppConfigConnectionStrings>();

        public string this[string key]
        {
            get { return ConfigurationManager.ConnectionStrings[key].ConnectionString; }
        }

        public bool Exists(string name)
        {
            return ConfigurationManager.ConnectionStrings["name"] != null;
        }

        public void Register(string name, string connectionString, string providerName = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {name} | {connectionString} | {providerName}", name, connectionString, providerName);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}