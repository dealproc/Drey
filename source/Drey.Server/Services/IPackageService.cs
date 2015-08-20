using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        Task<Models.Release> SyndicateAsync(Stream stream);
        Task<IEnumerable<Models.Package>> GetPackagesAsync();
        Task<IEnumerable<Models.Release>> GetReleasesAsync(string id);
        Task<Models.FileDownload> GetReleaseAsync(string id, string version);
        Task DeleteAsync(string id, string version);
    }
}