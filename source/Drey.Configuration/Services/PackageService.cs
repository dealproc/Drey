using Drey.Configuration.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Services
{
    public class PackageService : IPackageService
    {
        IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return _packageRepository.GetReleases(packageId);
        }

        public IEnumerable<DataModel.Package> GetPackages()
        {
            return _packageRepository.GetPackages();
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

        public void RecordReleases(IEnumerable<DataModel.Release> newReleases)
        {
            newReleases.Select(r => _packageRepository.Store(r)).ToList();
        }
    }
}