using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageRepository
    {
        IEnumerable<DataModel.RegisteredPackage> GetRegisteredPackages();
        DataModel.RegisteredPackage GetPackage(string packageId);
        void Store(DataModel.RegisteredPackage package);

        IEnumerable<DataModel.Release> GetReleases(string packageId);
    }
}