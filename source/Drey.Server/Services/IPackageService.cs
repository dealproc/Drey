using System.Collections.Generic;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        IEnumerable<Models.Release> GetReleases(string packageId);
        bool CreateRelease(string packageId, string fileName, System.IO.Stream stream);
    }
}