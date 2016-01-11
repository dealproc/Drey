using Drey.Logging;
using Drey.Nut;

using System;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;

namespace Drey
{
    /// <summary>
    /// Allows access to the app.config application settings
    /// </summary>
    public class AppConfigApplicationSettings : MarshalByRefObject, IApplicationSettings, IGlobalSettings
    {
        static ILog _log = LogProvider.For<AppConfigApplicationSettings>();

        /// <summary>
        /// Gets the application setting associated with the key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }

        /// <summary>
        /// Check to see if a given key exists within the application settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        /// <summary>
        /// Registers an application key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Register(string key, string value = "")
        {
            _log.WarnFormat("Register called for a non-implemented method. {key} | {value}", key, value);
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