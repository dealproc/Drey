using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageSettingRepository
    {
        IEnumerable<DataModel.PackageSetting> All();
        IEnumerable<DataModel.PackageSetting> All(string packageId);
        DataModel.PackageSetting Get(string packageId, string key);
        string ByKey(string packageId, string key);
        void Store(Services.ViewModels.AppSettingPmo model);
    }
}