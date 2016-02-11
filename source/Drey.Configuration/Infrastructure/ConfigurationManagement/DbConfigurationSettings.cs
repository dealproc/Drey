using Drey.Configuration.Repositories;
using Drey.Logging;
using Drey.Nut;
using Drey.Utilities;

using System;
using System.IO;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    /// <summary>
    /// Configuration manager instantiated and shared with packages in the runtime.  This facades the access to the configuration store, hard drive path(s), etc.
    /// </summary>
    public class DbConfigurationSettings : MarshalByRefObject, INutConfiguration, IDisposable
    {
        /// <summary>
        /// Factory delegate for registering with Autofac.   Allows autofac to create an instance of DbConfigurationSettings.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public delegate DbConfigurationSettings Factory(string packageId);

        static readonly ILog _log = LogProvider.For<DbConfigurationSettings>();

        readonly string _packageId;
        readonly INutConfiguration _hostApplicationConfiguration;
        readonly IGlobalSettingsRepository _globalSettingsRepository;
        Sponsor<IApplicationSettings> _applicationSettingsService;
        Sponsor<IConnectionStrings> _connectionStringsService;

        bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConfigurationSettings" /> class.
        /// </summary>
        /// <param name="hostApplicationConfiguration">The host application configuration.</param>
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
            _applicationSettingsService = new Sponsor<IApplicationSettings>(new Services.ApplicationSettingsService(_packageId, packageSettingRepository));
            _connectionStringsService = new Sponsor<IConnectionStrings>(new Services.ConnectionStringsService(_packageId, connectionStringsRepository));

            _log.Debug("Created db configuration settings provider");
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="DbConfigurationSettings"/> class.
        /// </summary>
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
            get { return _applicationSettingsService.Protege; }
        }

        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        public IConnectionStrings ConnectionStrings
        {
            get { return _connectionStringsService.Protege; }
        }

        /// <summary>
        /// Provides the folder where the runtime.exe file is located.
        /// </summary>
        public string InstallationDirectory { get { return _hostApplicationConfiguration.InstallationDirectory; } }

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
        /// Gets the base directory for the location that custom dlls/plugins are located.  This is a sub-folder of the WorkingDirectory folder.
        /// </summary>
        public string PluginsBaseDirectory { get { return Path.Combine(WorkingDirectory, "Plugins").NormalizePathSeparator(); } }

        /// <summary>
        /// Gets the logs directory.
        /// </summary>
        public string LogsDirectory { get { return Path.Combine(WorkingDirectory, "Logs").NormalizePathSeparator(); } }

        /// <summary>
        /// Gets an instance of a certificate validation routine, based on environment configuration.
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
        /// Gets a value indicating whether [shell controls lifetime sponsorship].
        /// </summary>
        /// <value>
        /// <c>true</c> if [shell controls lifetime sponsorship]; otherwise, <c>false</c>.
        /// </value>
        public bool ShellControlsLifetimeSponsorship { get { return true; } }

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

            if (_connectionStringsService != null)
            {
                _connectionStringsService.Dispose();
                _connectionStringsService = null;
            }

            if (_applicationSettingsService != null)
            {
                _applicationSettingsService.Dispose();
                _applicationSettingsService = null;
            }


            _disposed = true;
        }
    }
}