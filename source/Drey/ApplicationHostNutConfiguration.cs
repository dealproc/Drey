using Drey.Utilities;
using Drey.Nut;

using System;
using System.IO;
using System.Security.Permissions;

namespace Drey
{
    /// <summary>
    /// This is the configuration manager for the Console.  This should not be used for individual packages.
    /// </summary>
    public class ApplicationHostNutConfiguration : MarshalByRefObject, INutConfiguration
    {
        IApplicationSettings _applicationSettings;
        IGlobalSettings _globalSettings;
        IConnectionStrings _connectionStrings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationHostNutConfiguration"/> class.
        /// </summary>
        public ApplicationHostNutConfiguration()
        {
            var appSettings = new AppConfigApplicationSettings();
            _applicationSettings = appSettings;
            _globalSettings = appSettings;

            _connectionStrings = new AppConfigConnectionStrings();
        }

        /// <summary>
        /// Provides read access to a list of application settings that are valid for all executing packages in the runtime.
        /// </summary>
        public IGlobalSettings GlobalSettings
        {
            get { return _globalSettings; }
        }

        /// <summary>
        /// Provides read access to the application settings store.
        /// </summary>
        public IApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
        }

        /// <summary>
        /// Ability to retrieve one or more connection strings from the underlying connection string store.
        /// </summary>
        public IConnectionStrings ConnectionStrings
        {
            get { return _connectionStrings; }
        }

        /// <summary>
        /// Gets the base working directory.
        /// </summary>
        public string WorkingDirectory
        {
            get { return _applicationSettings["WorkingDirectory"].NormalizePathSeparator(); }
        }

        /// <summary>
        /// Gets the hoarde base directory, where all unpacked packages are stored for execution.
        /// </summary>
        public string HoardeBaseDirectory
        {
            get { return Path.Combine(WorkingDirectory, "Hoarde").NormalizePathSeparator(); }
        }

        /// <summary>
        /// Gets the logs directory, which is a subfolder of the main WorkingDirectory value.
        /// </summary>
        public string LogsDirectory { get { return Path.Combine(WorkingDirectory, "Logs").NormalizePathSeparator(); } }

        /// <summary>
        /// Gets or sets the system execution mode. (Production or Development).
        /// </summary>
        public ExecutionMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the log verbosity.
        /// </summary>
        public string LogVerbosity { get; set; }

        /// <summary>
        /// *Always* returns an Authority-Issued Certificate Validator.
        /// </summary>
        public CertificateValidation.ICertificateValidation CertificateValidator
        {
            // only ever return the authority issued certificate validation routine.
            get { return new CertificateValidation.AuthorityIssuedServerCertificateValidation(); }
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