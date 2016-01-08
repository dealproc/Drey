using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageRepository
    {
        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.Release> All();

        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.Package> GetPackages();
        
        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        IEnumerable<DataModel.Release> GetReleases(string packageId);

        /// <summary>
        /// Stores the release.
        /// </summary>
        /// <param name="release">The release.</param>
        /// <returns></returns>
        DataModel.Release Store(DataModel.Release release);

        /// <summary>
        /// Deletes the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="version">The version.</param>
        void Delete(string packageId, string version);
    }
}