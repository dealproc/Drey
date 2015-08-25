using System.Collections.Generic;

namespace Drey.Configuration.Repositories
{
    public interface IPackageRepository
    {
        IEnumerable<DataModel.Release> All();
        IEnumerable<DataModel.Package> GetPackages();
        IEnumerable<DataModel.Release> GetReleases(string packageId);
        DataModel.Release Store(DataModel.Release r);
    }
}