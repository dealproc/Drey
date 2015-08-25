using Drey.Configuration.Repositories;
using Drey.Nut;
using System;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    public class DbConfigurationSettings : MarshalByRefObject, Drey.Nut.INutConfiguration
    {
        readonly string _packageId;
        readonly IApplicationSettings _runtimeApplicationSettings;
        readonly IGlobalSettings _globalSettingsRepository;
        readonly IApplicationSettings _applicationSettingsService;
        readonly IConnectionStrings _connectionStringsService;

        public DbConfigurationSettings(IApplicationSettings runtimeApplicationSettings,
            IPackageSettingRepository packageSettingRepository,
            IConnectionStringRepository connectionStringsRepository,
            string packageId)
        {
            _runtimeApplicationSettings = runtimeApplicationSettings;
            _packageId = packageId;

            _globalSettingsRepository = new Repositories.SQLiteRepositories.GlobalSettingsRepository(this);
            _applicationSettingsService = new Services.ApplicationSettingsService(_packageId, packageSettingRepository);
            _connectionStringsService = new Services.ConnectionStringsService(_packageId, connectionStringsRepository);
        }

        public IGlobalSettings GlobalSettings
        {
            get { return _globalSettingsRepository; }
        }

        public IApplicationSettings ApplicationSettings
        {
            get { return _applicationSettingsService; }
        }

        public IConnectionStrings ConnectionStrings
        {
            get { return _connectionStringsService; }
        }

        public string HordeBaseDirectory
        {
            get { return _runtimeApplicationSettings["horde.directory"]; }
        }
    }
}