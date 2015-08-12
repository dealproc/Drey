using System.Collections.Generic;
using System.IO;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        IEnumerable<Models.Package> ListPackages();
        void DeletePackage(string p);
        IEnumerable<Models.Release> GetReleases(string packageId);
        bool CreateRelease(string packageId, string fileName, Stream stream);
        Models.FileDownload GetPackage(string sha);
    }
}