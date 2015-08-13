using System.Collections.Generic;
using System.IO;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        bool Exists(string packageId);
        IEnumerable<Models.Package> ListPackages();
        void DeletePackage(string p);
        IEnumerable<Models.Release> GetReleases(string packageId);
        bool CreateRelease(string packageId, string fileName, Stream stream);
        bool DeleteRelease(string sha);
        Models.FileDownload GetRelease(string sha);
    }
}