using Drey.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples.Server.Stores
{
    public class ReleasesStore : Drey.Server.Services.IReleaseStore
    {
        List<Release> _releases = new List<Release>();

        public Release Create()
        {
            return new Release();
        }

        public Task<Release> StoreAsync(Release model)
        {
            _releases.RemoveAll(r => r.Id.Equals(model.Id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(model.Version));
            _releases.Add(model);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<Package>> ListPackages()
        {
            return Task.FromResult(_releases
                .GroupBy(r => r.Id)
                .Select(rGrp => new Package
                {
                    Id = rGrp.Key,
                    Title = rGrp.OrderByDescending(item => item.Version).First().Title
                }
            ));
        }

        public Task<IEnumerable<Release>> ListAsync()
        {
            return Task.FromResult(_releases.AsEnumerable());
        }

        public Task<IEnumerable<Release>> ListByIdAsync(string id)
        {
            return Task.FromResult(_releases.Where(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<Release> GetAsync(string id, string version)
        {
            return Task.FromResult(_releases.FirstOrDefault(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(version)));
        }

        public Task DeleteAsync(string id, string version)
        {
            _releases.RemoveAll(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(version));
            return Task.FromResult(0);
        }
    }
}