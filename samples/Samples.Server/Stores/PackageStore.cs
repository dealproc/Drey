using Drey.Server.Models;
using Drey.Server.Services;
using System.Collections.Generic;
using System.Linq;

namespace Samples.Server.Stores
{
    public class PackageStore : IPackageStore
    {
        readonly List<Package> _packages;
        readonly IFileService _fileService;

        public PackageStore(IFileService fileService)
        {
            _fileService = fileService;

            _packages = new List<Package>();
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
