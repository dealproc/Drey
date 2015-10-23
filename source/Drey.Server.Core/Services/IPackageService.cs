using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        Task<Models.Release> SyndicateAsync(Stream stream);
        Task<IEnumerable<Models.Package>> GetPackagesAsync(ClaimsPrincipal principal = null);
        Task<IEnumerable<Models.Release>> GetReleasesAsync(string id, ClaimsPrincipal principal = null);
        Task<Models.FileDownload> GetReleaseAsync(string id, string version, ClaimsPrincipal principal = null);
        Task DeleteAsync(string id, string version);
    }
}