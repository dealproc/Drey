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
        IEnumerable<ViewModels.AppletInfoPmo> LatestRegisteredReleases();

        /// <summary>
        /// Stores a list of releases.
        /// </summary>
        /// <param name="newReleases">The new releases.</param>
        void RecordReleases(IEnumerable<DataModel.Release> newReleases);

        /// <summary>
        /// Renders a PMO to display the configuration of a package.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        ViewModels.AppletDashboardPmo Dashboard(string packageId);

        /// <summary>
        /// Gets the application setting from the repository.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        ViewModels.AppSettingPmo GetAppSetting(string packageId, string key);

        /// <summary>
        /// Records the application setting to the repository.
        /// </summary>
        /// <param name="model">The model.</param>
        void RecordAppSetting(ViewModels.AppSettingPmo model);

        /// <summary>
        /// Retrieves the connection string from the repository.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        ViewModels.ConnectionStringPmo GetConnectionString(string packageId, string name);

        /// <summary>
        /// Stores the connection string to the repository.
        /// </summary>
        /// <param name="model">The model.</param>
        void RecordConnectionString(ViewModels.ConnectionStringPmo model);

        /// <summary>
        /// Retrieves a list of the System.Data Connection Factory Providers registered in this system.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> ConnectionFactoryProviders();
    }
}