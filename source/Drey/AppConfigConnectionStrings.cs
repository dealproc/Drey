using Drey.Logging;
using Drey.Nut;
using System;
using System.Configuration;
using System.Security.Permissions;

namespace Drey
{
    /// <summary>
    /// Allows access to the connection strings element in the app.config of the runtime.
    /// </summary>
    public class AppConfigConnectionStrings : MarshalByRefObject, IConnectionStrings
    {
        static readonly ILog _log = LogProvider.For<AppConfigConnectionStrings>();

        /// <summary>
        /// Gets the connection string for the given key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return ConfigurationManager.ConnectionStrings[key].ConnectionString; }
        }

        /// <summary>
        /// Checks to see if a specific key exists in the underlying store.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            return ConfigurationManager.ConnectionStrings["name"] != null;
        }

        /// <summary>
        /// Registers a connection string, with a given "name" and provider name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        public void Register(string name, string connectionString, string providerName = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {name} | {connectionString} | {providerName}", name, connectionString, providerName);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// <remarks>We need to override the default functionality here and send back a `null` so that we can control the lifetime of the ServiceControl.  Default lease time is 5 minutes, which does not work for us.</remarks>
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}