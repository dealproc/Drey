using System.Collections.Generic;

namespace Drey.Server.Services
{
    public interface IPackageStore
    {
        void Store(Models.Package package);
        IEnumerable<Models.Package> Packages();
        IEnumerable<Models.Release> Releases(string packageId);
        void DeletePackage(string packageId);
        void DeleteRelease(string sha);
    }
}