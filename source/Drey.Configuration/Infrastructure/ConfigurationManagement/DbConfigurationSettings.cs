using Drey.Configuration.Repositories;
using Drey.Logging;
using Drey.Nut;
using Drey.Utilities;

using System;
using System.IO;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    /// <summary>
    /// Configuration manager instantiated and shared with packages in the runtime.  This facades the access to the configuration store, hard drive path(s), etc.
    /// </summary>
    public class DbConfigurationSettings : MarshalByRefObject, Drey.Nut.INutConfiguration, IDisposable
    {
        static readonly ILog _log = LogProvider.For<DbConfigurationSettings>();

        readonly string _packageId;
        readonly INutConfiguration _hostApplicationConfiguration;
        readonly IGlobalSettingsRepository _globalSettingsRepository;
        readonly IApplicationSettings _applicationSettingsService;
        readonly IConnectionStrings _connectionStringsService;

        bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConfigurationSettings"/> class.
        /// </summary>
        /// <param name="runtimeApplicationSettings">The runtime application settings.</param>
        /// <param name="packageSettingRepository">The package setting repository.</param>
        /// <param name="connectionStringsRepository">The connection strings repository.</param>
        /// <param name="packageId">The package identifier.</param>
        public DbConfigurationSettings(INutConfiguration hostApplicationConfiguration,
            IPackageSettingRepository packageSettingRepository,
            IConnectionStringRepository connectionStringsRepository,
            string packageId)
        {
            _hostApplicationConfiguration = hostApplicationConfiguration;
            _packageId = packageId;

            _globalSettingsRepository = new Repositories.SQLiteRepositories.GlobalSettingsRepository(this);
            _applicationSettingsService = new Services.ApplicationSettingsService(_packageId, packageSettingRepository);
            _connectionStringsService = new Services.ConnectionStringsService(_packageId, connectionStringsRepository);
        }
        ~DbConfigurationSettings()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the global settings.
        /// </summary>
        public IGlobalSettings GlobalSettings
        {
            get { return _globalSettingsRepository; }
        }

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        public IApplicationSettings ApplicationSettings
        {
            get { return _applicationSettingsService; }
        }

        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        public IConnectionStrings ConnectionStrings
        {
            get { return _connectionStringsService; }
        }

        /// <summary>
        /// Gets the Runtime's Working Directory.
        /// </summary>
        public string WorkingDirectory
        {
            get { return _hostApplicationConfiguration.WorkingDirectory; }
        }

        /// <summary>
        /// Gets the horde base directory.
        /// </summary>
        public string HoardeBaseDirectory { get { return Path.Combine(WorkingDirectory, "Hoarde").NormalizePathSeparator(); } }

        /// <summary>
        /// Gets the logs directory.
        /// </summary>
        public string LogsDirectory { get { return Path.Combine(WorkingDirectory, "Logs").NormalizePathSeparator(); } }

        /// <summary>
        /// Gets the certificate validator.
        /// </summary>
        public CertificateValidation.ICertificateValidation CertificateValidator
        {
            get
            {
                var thumbprint = GlobalSettings[DreyConstants.ServerSSLThumbprint] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(thumbprint))
                {
                    return new CertificateValidation.AuthorityIssuedServerCertificateValidation();
                }
                return new CertificateValidation.SelfSignedServerCertificateValidation(thumbprint);
            }
        }

        /// <summary>
        /// Gets the mode that the runtime is executing in.  Possible values are Development and Production.
        /// </summary>
        public ExecutionMode Mode
        {
            get { return _hostApplicationConfiguration.Mode; }
        }

        /// <summary>
        /// Gets the log verbosity.
        /// </summary>
        public string LogVerbosity
        {
            get { return _hostApplicationConfiguration.LogVerbosity; }
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
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
            //
            // Returning null designates an infinite non-expiring lease.
            // We must therefore ensure that RemotingServices.Disconnect() is called when
            // it's no longer needed otherwise there will be a memory leak.
            //
            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            RemotingServices.Disconnect(this);

            _disposed = true;
        }
    }
}