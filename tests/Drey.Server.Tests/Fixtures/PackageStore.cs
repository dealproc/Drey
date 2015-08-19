using Drey.Server.Models;
using Drey.Server.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drey.Server.Tests.Fixtures
{
    public class PackageStore : IPackageStore
    {
        readonly List<Package> _packages;
        readonly IFileService _fileService;

        public PackageStore(IFileService fileService)
        {
            _fileService = fileService;

            _packages = new List<Package>
            {
                new Package
                { 
                    PackageId = "test.package", 
                    Releases = new List<Release>
                    {
                        new Release{ Filename = "package-1.0.0.0.zip", Filesize = Resources.Files.validzipfile.Length, SHA1 = Utilities.CalculateChecksum(new MemoryStream(Resources.Files.validzipfile)) }
                    }
                }
            };
            var filename = _fileService.StoreAsync("package-1.0.0.0.zip", new MemoryStream(Resources.Files.validzipfile)).Result;
        }

        public void Store(Package package)
        {
            if (_packages.Any(p => p.PackageId == package.PackageId))
            {
                _packages.RemoveAll(p => p.PackageId == package.PackageId);
            }
            _packages.Add(package);
        }

        public IEnumerable<Package> Packages()
        {
            return _packages;
        }

        public IEnumerable<Release> Releases(string packageId)
        {
            var package = _packages.Where(p => p.PackageId == packageId).FirstOrDefault();
            if (package == null) { throw new KeyNotFoundException("packageId is invalid."); }

            return package.Releases;
        }

        public void DeletePackage(string packageId)
        {
            _packages.RemoveAll(p => p.PackageId == packageId);
        }

        public void DeleteRelease(string sha)
        {
            foreach (var pkg in _packages)
            {
                var releasesToDelete = pkg.Releases.Where(r => r.SHA1 == sha).ToArray();
                foreach (var release in releasesToDelete)
                {
                    pkg.Releases.Remove(release);
                }
            }
        }
    }
}