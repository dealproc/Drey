using System.Collections.Generic;

namespace Drey.Server.Services
{
    public interface IPackageStore
    {
        IEnumerable<Models.PackageInfo> ListPackages();
        Models.PackageDetails GetPackage(string packageId);
    }
}