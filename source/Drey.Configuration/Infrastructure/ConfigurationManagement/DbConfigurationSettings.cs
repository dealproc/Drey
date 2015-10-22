﻿using Drey.Configuration.Repositories;
using Drey.Nut;
using Drey.Utilities;

using System;
using System.IO;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    public class DbConfigurationSettings : MarshalByRefObject, Drey.Nut.INutConfiguration
    {
        readonly string _packageId;
        readonly INutConfiguration _hostApplicationConfiguration;
        readonly IGlobalSettingsRepository _globalSettingsRepository;
        readonly IApplicationSettings _applicationSettingsService;
        readonly IConnectionStrings _connectionStringsService;

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

        public string WorkingDirectory
        {
            get
            {
                var settings = _hostApplicationConfiguration.ApplicationSettings;
                var dir = settings["WorkingDirectory"].NormalizePathSeparator();
                return dir;
            }
        }

        /// <summary>
        /// Gets the horde base directory.
        /// </summary>
        public string HordeBaseDirectory { get { return Path.Combine(WorkingDirectory, "Hoarde").NormalizePathSeparator(); } }

        public string LogsDirectory { get { return Path.Combine(WorkingDirectory, "Logs").NormalizePathSeparator(); } }


        public ExecutionMode Mode
        {
            get { return _hostApplicationConfiguration.Mode; }
        }
    }
}