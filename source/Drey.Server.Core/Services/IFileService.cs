using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Stores the provided string with the provided filename.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        Task<string> StoreAsync(string fileName, Stream stream);

        /// <summary>
        /// Downloads the file from the storage medium.
        /// </summary>
        /// <param name="filename">The uri to access the file.</param>
        /// <returns>A stream which contains the file data</returns>
        Task<Stream> DownloadBlobAsync(string filename);

        /// <summary>
        /// Enumerates the files on the storage medium.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> EnumerateFilesAsync();

        /// <summary>
        /// Deletes a file from the storage medium.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string filename);
    }
}