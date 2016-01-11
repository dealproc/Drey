using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IPackageService
    {
        /// <summary>
        /// Stores a package from a stream to an underlying filestore and extracts out the package details for publication.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        Task<Models.Release> SyndicateAsync(Stream stream);
        
        /// <summary>
        /// Retrieves a list of previously syndicated packages for display.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <returns></returns>
        Task<IEnumerable<Models.Package>> GetPackagesAsync(ClaimsPrincipal principal = null);
        
        /// <summary>
        /// Gets a list of known releases of a particular package, filtered by the presented ClaimsPrincipal, if warranted.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="principal">The principal.</param>
        /// <returns></returns>
        Task<IEnumerable<Models.Release>> GetReleasesAsync(string id, ClaimsPrincipal principal = null);
        
        /// <summary>
        /// Gets the release from the underlying filestore based on its id and version, and sets up the necessary information to allow a runtime client to download it.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="version">The version.</param>
        /// <param name="principal">The principal.</param>
        /// <returns></returns>
        Task<Models.FileDownload> GetReleaseAsync(string id, string version, ClaimsPrincipal principal = null);
        
        /// <summary>
        /// Deletes a package from the underlying filestore.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        Task DeleteAsync(string id, string version);
    }
}