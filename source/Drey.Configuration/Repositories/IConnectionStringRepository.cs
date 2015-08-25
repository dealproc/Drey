using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IConnectionStringRepository
    {
        IEnumerable<DataModel.PackageConnectionString> All();
        IEnumerable<DataModel.PackageConnectionString> All(string packageId);
        string ByKey(string packageId, string key);
    }
}