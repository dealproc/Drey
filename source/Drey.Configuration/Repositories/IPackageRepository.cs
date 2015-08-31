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
        /// Stores the specified r.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        DataModel.Release Store(DataModel.Release r);
    }
}