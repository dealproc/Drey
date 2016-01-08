using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageSettingRepository
    {
        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataModel.PackageSetting> All();
        
        /// <summary>
        /// Alls the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        IEnumerable<DataModel.PackageSetting> All(string packageId);
        
        /// <summary>
        /// Gets the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        DataModel.PackageSetting Get(string packageId, string key);
        
        /// <summary>
        /// Retrieves a stored setting by its Package Id and Key.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string ByKey(string packageId, string key);
        
        /// <summary>
        /// Stores the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        void Store(Services.ViewModels.AppSettingPmo model);

        void Store(DataModel.PackageSetting model);

        /// <summary>
        /// Deletes the specified package setting, by its key.
        /// </summary>
        /// <param name="id">The key of the package setting.</param>
        void Delete(int id);
    }
}