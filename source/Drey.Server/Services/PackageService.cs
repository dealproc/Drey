using System.Collections.Generic;
using System.Linq;

namespace Drey.Server.Services
{
    public class PackageService : IPackageService
    {
        readonly IPackageStore _packageStore;
        readonly IFileService _fileService;

        public PackageService(IPackageStore packageStore, IFileService fileService)
        {
            _packageStore = packageStore;
            _fileService = fileService;
        }
        public bool Exists(string packageId)
        {
            return _packageStore.Packages().Any(p => p.PackageId == packageId);
        }

        public IEnumerable<Models.Package> ListPackages()
        {
            return _packageStore.Packages();
        }

        public void DeletePackage(string packageId)
        {
            _packageStore.DeletePackage(packageId);
        }

        public IEnumerable<Models.Release> GetReleases(string packageId)
        {
            return _packageStore.Releases(packageId);
        }

        public bool CreateRelease(string packageId, string fileName, System.IO.Stream stream)
        {
            if (string.IsNullOrEmpty(packageId)) { return false; }
            if (stream == null || stream.Length == 0) { return false; }

            var storedFileReference = _fileService.StoreAsync(fileName, stream).Result;

            Models.Package package;

            if ((package = _packageStore.Packages().SingleOrDefault(p => p.PackageId == packageId)) == null)
            {
                package = new Models.Package { PackageId = packageId };
                _packageStore.Store(package);
            }

            string checksum = Utilities.CalculateChecksum(stream);
            package.Releases.Add(new Models.Release { Filename = storedFileReference, Filesize = stream.Length, SHA1 = checksum });
            _packageStore.Store(package);

            return true;
        }

        public Models.FileDownload GetRelease(string sha)
        {
            var release = _packageStore.Packages().SelectMany(x => x.Releases.Where(r => r.SHA1 == sha)).FirstOrDefault();
            if (release == null) { throw new KeyNotFoundException("SHA does not exist."); }

            var contents = _fileService.DownloadBlobAsync(release.Filename).Result;

            return new Models.FileDownload
            {
                FileContents = contents,
                Filename = release.Filename,
                MimeType = "octet/stream"
            };
        }

        public bool DeleteRelease(string sha)
        {
            var release = _packageStore.Packages().SelectMany(x => x.Releases.Where(r => r.SHA1 == sha)).FirstOrDefault();
            if (release == null) { throw new KeyNotFoundException("SHA does not exist."); }

            var wasSuccessful = _fileService.DeleteAsync(release.Filename).Result;
            _packageStore.DeleteRelease(sha);

            return true;
        }
    }
}