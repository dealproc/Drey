using NuGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IReleaseStore
    {
        /// <summary>
        /// Creates a reference object.
        /// <remarks>This is useful if you will be inheriting the <see cref="Models.Release"/> to extend it for storage in say, a sql server store.</remarks>
        /// </summary>
        /// <returns></returns>
        Models.Release Create();

        /// <summary>
        /// Stores a release to permanent storage.
        /// </summary>
        /// <param name="model">The release details.</param>
        Task<Models.Release> StoreAsync(Models.Release model);

        /// <summary>
        /// Lists the packages.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Models.Package>> ListPackages();

        /// <summary>
        /// Lists all known releases of all stored packages.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Models.Release>> ListAsync();

        /// <summary>
        /// Produces a list of releases, by package id.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <returns></returns>
        Task<IEnumerable<Models.Release>> ListByIdAsync(string id);

        /// <summary>
        /// Gets information about a specific package/version.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="version">The version of the package.</param>
        /// <returns></returns>
        Task<Models.Release> GetAsync(string id, string version);

        /// <summary>
        /// Deletes a specific package/version combination from the store.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="version">The version of the package.</param>
        /// <returns></returns>
        Task DeleteAsync(string id, string version);
    }
}