using Drey.Configuration.Repositories;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Drey.Configuration.Services
{
    public class PackageService : IPackageService
    {
        IPackageRepository _packageRepository;
        IConnectionStringRepository _connectionStringRepository;
        IPackageSettingRepository _packageSettingRepository;

        public PackageService(IPackageRepository packageRepository, IConnectionStringRepository connectionStringRepository, IPackageSettingRepository packageSettingRepository)
        {
            _packageRepository = packageRepository;
            _connectionStringRepository = connectionStringRepository;
            _packageSettingRepository = packageSettingRepository;
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return _packageRepository.GetReleases(packageId);
        }

        public IEnumerable<DataModel.Release> LatestRegisteredReleases()
        {
            return _packageRepository.All()
                .Select(rel => new { Release = rel, Version = new NuGet.SemanticVersion(rel.Version) })
                .GroupBy(x => x.Release.Id)
                .Select(x => x.OrderByDescending(rel => rel.Version).First().Release);
        }

        public IEnumerable<DataModel.Package> GetPackages()
        {
            return _packageRepository.GetPackages();
        }

        /// <summary>
        /// Discovers releases that have not been applied to this client.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="discoveredReleases">The discovered releases.</param>
        /// <returns>A list of <see cref="DataModel.Release"/> that have not been applied</returns>
        public IEnumerable<DataModel.Release> Diff(string packageId, IEnumerable<DataModel.Release> discoveredReleases)
        {
            var appliedSHAs = _packageRepository.GetReleases(packageId).Select(x => x.SHA1).ToArray();
            return discoveredReleases.Where(d => !appliedSHAs.Contains(d.SHA1));
        }

        public void RecordReleases(IEnumerable<DataModel.Release> newReleases)
        {
            newReleases.Select(r => _packageRepository.Store(r)).ToList();
        }

        public ViewModels.AppletDashboardPmo Dashboard(string packageId)
        {
            var release = _packageRepository.GetReleases(packageId)
                .Select(x => new { Release = x, Version = new NuGet.SemanticVersion(x.Version) })
                .OrderByDescending(x => x.Version)
                .First().Release;

            return new ViewModels.AppletDashboardPmo
            {
                Id = release.Id,
                Title = release.Title,
                Version = release.Version,
                AppSettings = _packageSettingRepository.All(packageId).Select(setting => new ViewModels.AppletDashboardPmo.AppletSetting { Id = setting.Id, Key = setting.Key, Value = setting.Value }),
                ConnectionStrings = _connectionStringRepository.All(packageId).Select(cn => new ViewModels.AppletDashboardPmo.AppletConnectionString { Id = cn.Id, Name = cn.Name, ConnectionString = cn.ConnectionString, ProviderName = cn.ProviderName })
            };
        }


        public ViewModels.AppSettingPmo GetAppSetting(string packageId, string key)
        {
            var dbAppSetting = _packageSettingRepository.Get(packageId, key);
            return dbAppSetting != null
                ?
                new ViewModels.AppSettingPmo { Key = dbAppSetting.Key, Value = dbAppSetting.Value, PackageId = dbAppSetting.PackageId }
                :
                default(ViewModels.AppSettingPmo);
        }

        public void RecordAppSetting(ViewModels.AppSettingPmo model)
        {
            _packageSettingRepository.Store(model);
        }

        public ViewModels.ConnectionStringPmo GetConnectionString(string packageId, string name)
        {
            var dbConnStr = _connectionStringRepository.Get(packageId, name);
            return dbConnStr != null
                ?
                new ViewModels.ConnectionStringPmo { Name = dbConnStr.Name, ConnectionString = dbConnStr.ConnectionString, ProviderName = dbConnStr.ProviderName, PackageId = dbConnStr.PackageId, Providers = this.ConnectionFactoryProviders() }
                :
                default(ViewModels.ConnectionStringPmo);
        }

        public void RecordConnectionString(ViewModels.ConnectionStringPmo model)
        {
            _connectionStringRepository.Store(model);
        }

        public IDictionary<string, string> ConnectionFactoryProviders()
        {
            return DbProviderFactories.GetFactoryClasses().Select().ToDictionary(dr => dr["Name"].ToString(), dr => dr["InvariantName"].ToString());
        }
    }
}