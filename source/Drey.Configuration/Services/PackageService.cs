using Drey.Configuration.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Drey.Configuration.Services
{
    public class PackageService : IPackageService
    {
        readonly IPackageRepository _packageRepository;
        readonly IConnectionStringRepository _connectionStringRepository;
        readonly IPackageSettingRepository _packageSettingRepository;
        readonly ServiceModel.IHoardeManager _hoardManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageService"/> class.
        /// </summary>
        /// <param name="packageRepository">The package repository.</param>
        /// <param name="connectionStringRepository">The connection string repository.</param>
        /// <param name="packageSettingRepository">The package setting repository.</param>
        public PackageService(IPackageRepository packageRepository, IConnectionStringRepository connectionStringRepository, IPackageSettingRepository packageSettingRepository, ServiceModel.IHoardeManager hoardeManager)
        {
            _packageRepository = packageRepository;
            _connectionStringRepository = connectionStringRepository;
            _packageSettingRepository = packageSettingRepository;
            _hoardManager = hoardeManager;
        }

        /// <summary>
        /// Gets the list of historical releases, for a given package id.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return _packageRepository.GetReleases(packageId);
        }

        /// <summary>
        /// Retrieves the list of latest releases registered on the system.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ViewModels.AppletInfoPmo> LatestRegisteredReleases()
        {
            var releases = _packageRepository.All()
                .Select(rel => new { Release = rel, Version = new NuGet.SemanticVersion(rel.Version) })
                .GroupBy(x => x.Release.Id.ToLower())
                .Select(x => x.OrderByDescending(rel => rel.Version).First().Release);

            var models = releases.Select(x => new ViewModels.AppletInfoPmo
            {
                Description = x.Description,
                Id = x.Id,
                UpdatedOn = x.UpdatedOn,
                ReleaseNotes = x.ReleaseNotes,
                SHA1 = x.SHA1,
                Online = _hoardManager.IsOnline(x),
                Summary = x.Summary,
                Title = x.Title,
                Version = x.Version
            });

            return models;
        }

        /// <summary>
        /// Gets a list of packages.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Stores a list of releases.
        /// </summary>
        /// <param name="newReleases">The new releases.</param>
        public void RecordReleases(IEnumerable<DataModel.Release> newReleases)
        {
            newReleases.Select(r => _packageRepository.Store(r)).ToList();
        }

        /// <summary>
        /// Renders a PMO to display the configuration of a package.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the application setting from the repository.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ViewModels.AppSettingPmo GetAppSetting(string packageId, string key)
        {
            var dbAppSetting = _packageSettingRepository.Get(packageId, key);
            return dbAppSetting != null
                ?
                new ViewModels.AppSettingPmo { Id = dbAppSetting.Id, Key = dbAppSetting.Key, Value = dbAppSetting.Value, PackageId = dbAppSetting.PackageId }
                :
                default(ViewModels.AppSettingPmo);
        }

        /// <summary>
        /// Records the application setting to the repository.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RecordAppSetting(ViewModels.AppSettingPmo model)
        {
            _packageSettingRepository.Store(model);
        }

        /// <summary>
        /// Removes the application setting from the underlying repository.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RemoveAppSetting(ViewModels.AppSettingPmo model)
        {
            _packageSettingRepository.Delete(model.Id);
        }

        /// <summary>
        /// Retrieves the connection string from the repository.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ViewModels.ConnectionStringPmo GetConnectionString(string packageId, string name)
        {
            var dbConnStr = _connectionStringRepository.Get(packageId, name);
            return dbConnStr != null
                ?
                new ViewModels.ConnectionStringPmo { Id = dbConnStr.Id, Name = dbConnStr.Name, ConnectionString = dbConnStr.ConnectionString, ProviderName = dbConnStr.ProviderName, PackageId = dbConnStr.PackageId, Providers = this.ConnectionFactoryProviders() }
                :
                default(ViewModels.ConnectionStringPmo);
        }

        /// <summary>
        /// Stores the connection string to the repository.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RecordConnectionString(ViewModels.ConnectionStringPmo model)
        {
            _connectionStringRepository.Store(model);
        }

        /// <summary>
        /// Removes the connection string from the underlying repository.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RemoveConnectionString(ViewModels.ConnectionStringPmo model)
        {
            _connectionStringRepository.Delete(model.Id);
        }


        /// <summary>
        /// Retrieves a list of the System.Data Connection Factory Providers registered in this system.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> ConnectionFactoryProviders()
        {
            return DbProviderFactories.GetFactoryClasses().Select().ToDictionary(dr => dr["Name"].ToString(), dr => dr["InvariantName"].ToString());
        }
    }
}