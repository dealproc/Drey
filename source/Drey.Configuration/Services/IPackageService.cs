using System;
using System.Collections.Generic;
namespace Drey.Configuration.Services
{
    public interface IPackageService
    {
        /// <summary>
        /// Differences the list of retrieved releases with the known set of releases within the system.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="discoveredReleases">The discovered releases.</param>
        /// <returns></returns>
        IEnumerable<DataModel.Release> Diff(string packageId, IEnumerable<DataModel.Release> discoveredReleases);

        /// <summary>
        /// Gets a list of packages.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.Package> GetPackages();

        /// <summary>
        /// Gets the list of historical releases, for a given package id.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        IEnumerable<DataModel.Release> GetReleases(string packageId);

        /// <summary>
        /// Retrieves the list of latest releases registered on the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.Release> LatestRegisteredReleases();

        /// <summary>
        /// Stores a list of releases.
        /// </summary>
        /// <param name="newReleases">The new releases.</param>
        void RecordReleases(System.Collections.Generic.IEnumerable<DataModel.Release> newReleases);

        /// <summary>
        /// Renders a PMO to display the configuration of a package.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        ViewModels.AppletDashboardPmo Dashboard(string packageId);

        /// <summary>
        /// Gets the application setting.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        ViewModels.AppSettingPmo GetAppSetting(string packageId, string key);

        /// <summary>
        /// Records the application setting.
        /// </summary>
        /// <param name="model">The model.</param>
        void RecordAppSetting(ViewModels.AppSettingPmo model);

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        ViewModels.ConnectionStringPmo GetConnectionString(string packageId, string name);

        /// <summary>
        /// Records the connection string.
        /// </summary>
        /// <param name="model">The model.</param>
        void RecordConnectionString(ViewModels.ConnectionStringPmo model);

        /// <summary>
        /// Connections the factory providers.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> ConnectionFactoryProviders();
    }
}