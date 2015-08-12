using Drey.Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Drey.Server.Tests.Services
{
    class InMemoryPackageService : IPackageService
    {
        List<Models.Package> _packages;
        IFileService _fileService;
        public InMemoryPackageService()
        {
            _packages = new List<Models.Package>()
            {
                new Models.Package
                { 
                    PackageId = "test.package", 
                    Releases = new List<Models.Release>
                    {
                        new Models.Release{ Filename = "package-1.0.0.0.zip", Filesize = Samples.Server.Services.InMemory.Resources.Files.validzipfile.Length, SHA1 = CalculateChecksum(new MemoryStream(Samples.Server.Services.InMemory.Resources.Files.validzipfile)) }
                    }
                }
            };
            _fileService = new FilesytemFileService(@"c:\packages_test");
            var filename = _fileService.StoreAsync("package-1.0.0.0.zip", new MemoryStream(Samples.Server.Services.InMemory.Resources.Files.validzipfile)).Result;
        }

        public IEnumerable<Models.Package> ListPackages()
        {
            return _packages;
        }

        public void DeletePackage(string packageId)
        {
            _packages.RemoveAll(p => p.PackageId.Equals(packageId));
        }

        public IEnumerable<Models.Release> GetReleases(string packageId)
        {
            if (!_packages.Any(x => x.PackageId == packageId))
            {
                throw new KeyNotFoundException("Unknown package Id.");
            }
            var package = _packages.Single(x => x.PackageId == packageId);
            return package.Releases;
        }

        public bool CreateRelease(string packageId, string fileName, System.IO.Stream stream)
        {
            if (string.IsNullOrEmpty(packageId)) { return false; }
            if (stream == null || stream.Length == 0) { return false; }

            var storedFileReference = _fileService.StoreAsync(fileName, stream).Result;

            Models.Package package;
            if ((package = _packages.SingleOrDefault(p => p.PackageId == packageId)) == null)
            {
                package = new Models.Package() { PackageId = packageId };
                _packages.Add(package);
            }

            string checksum = CalculateChecksum(stream);
            package.Releases.Add(new Models.Release { Filename = storedFileReference, Filesize = stream.Length, SHA1 = checksum });

            return true;
        }

        private static string CalculateChecksum(System.IO.Stream stream)
        {
            SHA1Managed sha = new SHA1Managed();
            byte[] checksum = sha.ComputeHash(stream);
            string checksumStr = BitConverter.ToString(checksum).Replace("-", string.Empty).ToUpper();
            return checksumStr;
        }

        public Models.FileDownload GetRelease(string sha)
        {
            var releaseInfo = _packages.SelectMany(p => p.Releases.Where(r => r.SHA1 == sha)).FirstOrDefault();
            if (releaseInfo == null)
            {
                throw new KeyNotFoundException("SHA does not exist.");
            }

            var fileContents = _fileService.DownloadBlobAsync(releaseInfo.Filename).Result;
            return new Models.FileDownload
            {
                FileContents = fileContents,
                Filename = releaseInfo.Filename,
                MimeType = "octet/stream"
            };
        }

        public bool Exists(string packageId)
        {
            return _packages.Any(p => p.PackageId.Equals(packageId));
        }
    }
}