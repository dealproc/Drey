using Drey.Server.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Tests.Fixtures
{
    public class ReleasesStorage : IReleaseStore
    {
        readonly string test_package = "test.package.1.0.0.0.nupkg";
        readonly IFileService _fileService;
        readonly List<Models.Release> _releases;

        public ReleasesStorage(IFileService fileService)
        {
            _fileService = fileService;

            if (File.Exists(Path.Combine(TestNancyBootstrapper.TEST_PACKAGE_DIR, test_package)))
            {
                File.WriteAllBytes(Path.Combine(TestNancyBootstrapper.TEST_PACKAGE_DIR, test_package), Resources.Files.validzipfile);
            }
            _releases = new List<Models.Release> 
            {
                new Models.Release { Id = "test.package", Version = "1.0.0.0", Description = "A package for testing purposes.", Title = "A Test Package", RelativeUri = test_package }
            };
        }

        public Models.Release Create()
        {
            return new Models.Release();
        }

        public Task<Models.Release> StoreAsync(Models.Release model)
        {
            _releases.RemoveAll(r => r.Id.Equals(model.Id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(model.Version));
            _releases.Add(model);
            return Task.FromResult(model);
        }

        public Task<IEnumerable<Models.Package>> ListPackages(ClaimsPrincipal principal = null)
        {
            return Task.FromResult(_releases
                .GroupBy(r => r.Id)
                .Select(rGrp => new Models.Package
                {
                    Id = rGrp.Key,
                    Title = rGrp.OrderByDescending(item => item.Version).First().Title
                }
            ));
        }

        public Task<IEnumerable<Models.Release>> ListAsync(ClaimsPrincipal principal = null)
        {
            return Task.FromResult(_releases.AsEnumerable());
        }

        public Task<IEnumerable<Models.Release>> ListByIdAsync(string id, ClaimsPrincipal principal = null)
        {
            if (id.Equals("exception", StringComparison.InvariantCultureIgnoreCase)) { throw new Exception("test exception."); }

            return Task.FromResult(_releases.Where(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<Models.Release> GetAsync(string id, string version, ClaimsPrincipal principal = null)
        {
            if (id.Equals("exception", StringComparison.InvariantCultureIgnoreCase)) { throw new Exception("test exception"); }
            return Task.FromResult(_releases.FirstOrDefault(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(version)));
        }

        public Task DeleteAsync(string id, string version)
        {
            _releases.RemoveAll(r => r.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) && r.Version.Equals(version));
            return Task.FromResult(0);
        }
    }
}