using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IConnectionStringRepository
    {
        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.PackageConnectionString> All();

        /// <summary>
        /// Alls the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        IEnumerable<DataModel.PackageConnectionString> All(string packageId);
        
        /// <summary>
        /// Gets the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        DataModel.PackageConnectionString Get(string packageId, string name);
        
        /// <summary>
        /// Bies the name.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string ByName(string packageId, string name);
        
        /// <summary>
        /// Stores the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        void Store(Services.ViewModels.ConnectionStringPmo model);
    }
}