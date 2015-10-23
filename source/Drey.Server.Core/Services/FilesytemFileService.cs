using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public class FilesytemFileService : IFileService
    {
        string _baseFolder;

        public FilesytemFileService(string baseFolder)
        {
            _baseFolder = baseFolder;

            if (!Directory.Exists(_baseFolder))
            {
                Directory.CreateDirectory(_baseFolder);
            }
        }

        /// <summary>
        /// Stores the provided string with the provided filename.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<string> StoreAsync(string fileName, System.IO.Stream stream)
        {
            var fileNameAndPath = Path.Combine(_baseFolder, fileName);
            if (!Directory.Exists(_baseFolder))
            {
                Directory.CreateDirectory(_baseFolder);
            }
            using (var file = System.IO.File.OpenWrite(fileNameAndPath))
            {
                await stream.CopyToAsync(file);
            }

            return fileName;
        }

        /// <summary>
        /// Downloads the file from the storage medium.
        /// </summary>
        /// <param name="filename">The uri to access the file.</param>
        /// <returns>
        /// A stream which contains the file data
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<System.IO.Stream> DownloadBlobAsync(string filename)
        {
            var fullNameAndPath = Path.Combine(_baseFolder, filename);
            return Task.FromResult(
                File.Exists(fullNameAndPath) ?
                    (Stream)File.OpenRead(fullNameAndPath)
                    :
                    default(Stream)
            );
        }

        /// <summary>
        /// Enumerates the files on the storage medium.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<string>> EnumerateFilesAsync()
        {
            if (!Directory.Exists(_baseFolder)) { return Task.FromResult(Enumerable.Empty<string>()); }

            var files = Directory.GetFiles(_baseFolder)
                .Select(x =>
                {
                    var uri = x.Substring(_baseFolder.Length);
                    if (uri.StartsWith("\\") || uri.StartsWith("/"))
                    {
                        uri = uri.Substring(1);
                    };
                    return uri;
                })
                .OrderBy(x => x)
                .AsEnumerable();

            return Task.FromResult(files);
        }

        /// <summary>
        /// Deletes a file from the storage medium.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> DeleteAsync(string filename)
        {
            var fullPath = Path.Combine(_baseFolder, filename.Replace('/', '\\'));
            if (!Directory.Exists(_baseFolder)) return Task.FromResult(true);

            var fInfo = new FileInfo(fullPath);
            if (fInfo.Exists)
            {
                fInfo.Delete();
            }
            if (!fInfo.Directory.GetFiles().Any())
            {
                fInfo.Directory.Delete(true);
            }
            return Task.FromResult(true);
        }
    }
}