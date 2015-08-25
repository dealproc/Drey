using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IConnectionStringRepository
    {
        IEnumerable<DataModel.PackageConnectionString> All();
        IEnumerable<DataModel.PackageConnectionString> All(string packageId);
        DataModel.PackageConnectionString Get(string packageId, string name);
        string ByName(string packageId, string name);
        void Store(Services.ViewModels.ConnectionStringPmo model);
    }
}