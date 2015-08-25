using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageSettingRepository
    {
        IEnumerable<DataModel.PackageSetting> All();
        IEnumerable<DataModel.PackageSetting> All(string packageId);
        string ByKey(string _packageId, string key);
    }
}