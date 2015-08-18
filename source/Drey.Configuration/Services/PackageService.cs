using Drey.Configuration.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Services
{
    public class PackageService
    {
        IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public IEnumerable<DataModel.RegisteredPackage> GetRegisteredPackages()
        {
            return _packageRepository.GetRegisteredPackages();
        }

        public IEnumerable<DataModel.RegisteredPackage> RegisterNewPackages(IEnumerable<DataModel.RegisteredPackage> packages)
        {
            var knownPackages = GetRegisteredPackages().Select(p => p.PackageId).ToArray();
            var toRegister = packages.Where(p => !knownPackages.Contains(p.PackageId)).ToArray();
            return toRegister.Select(r => Register(r.PackageId));
        }

        public DataModel.RegisteredPackage Register(string packageId)
        {
            DataModel.RegisteredPackage package = null;

            if ((package = _packageRepository.GetPackage(packageId)) == null)
            {
                package = new DataModel.RegisteredPackage { PackageId = packageId };
                _packageRepository.Store(package);
            }

            return package;
        }

        public DataModel.RegisteredPackage GetPackage(string packageId)
        {
            return _packageRepository.GetPackage(packageId);
        }

        /// <summary>
        /// Discovers releases that have not been applied to this client.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="discoveredReleases">The discovered releases.</param>
        /// <returns>A list of <see cref="DataModel.Release"/> that have not been applied</returns>
        public IEnumerable<DataModel.Release> Diff(string packageId, IEnumerable<DataModel.Release> discoveredReleases)
        {
            var appliedSHAs = _packageRepository.GetReleases(packageId).Select(x => x.SHA1).ToArray();
            return discoveredReleases.Where(d => !appliedSHAs.Contains(d.SHA1));
        }

        public IEnumerable<DataModel.Release> GetReleases(DataModel.RegisteredPackage package)
        {
            return _packageRepository.GetReleases(package.PackageId);
        }

        public void RecordReleases(IEnumerable<DataModel.Release> newReleases)
        {
            newReleases.Select(r => _packageRepository.Store(r)).ToList();
        }
    }
}