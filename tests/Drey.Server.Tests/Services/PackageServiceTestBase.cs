using Drey.Server.Services;

using FakeItEasy;

using Shouldly;

using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Services
{
    [Collection("Package Management")]
    public class PackageServiceTestBase
    {
        IReleaseStore _releaseStore;
        IFileService _fileService;
        IPackageService _SUT;

        public PackageServiceTestBase()
        {
            _releaseStore = A.Fake<IReleaseStore>(opts => opts.Strict());
            _fileService = A.Fake<IFileService>(opts => opts.Strict());

            _SUT = new PackageService(_releaseStore, _fileService);
        }

        [Fact]
        public async Task CanRetrievePackagesAsync()
        {
            A.CallTo(() => _releaseStore.ListPackages(A<ClaimsPrincipal>.Ignored)).Returns(new[] { new Models.Package { } });

            var listOfPackages = await _SUT.GetPackagesAsync();

            listOfPackages.ShouldNotBe(null);
            listOfPackages.Count().ShouldBe(1);
        }

        [Fact]
        public async Task CanQueryReleasesAsync()
        {
            string test_package_id = "test.package";
            A.CallTo(() => _releaseStore.ListByIdAsync(A<string>.That.IsEqualTo(test_package_id), A<ClaimsPrincipal>.Ignored))
                .Returns(
                new[]
                {
                    new Models.Release{ Id = test_package_id, Version = "1.0.0.0" },
                    new Models.Release{ Id = test_package_id, Version = "1.1.0.0" },
                    new Models.Release{ Id = test_package_id, Version = "1.2.0.0" },
                });

            var releases = await _SUT.GetReleasesAsync(test_package_id);

            releases.ShouldNotBe(null);
            releases.Count().ShouldBe(3);
        }

        [Fact]
        public async Task CanRetrieveAFileFromStorageAsync()
        {
            string packageId = "test.package";
            string version = "1.0.0.0";
            string relative_url = string.Format("{0}.{1}.nupkg", packageId, version);

            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored, A<ClaimsPrincipal>.Ignored)).Returns(new Models.Release { Id = packageId, Version = version, RelativeUri = relative_url });
            A.CallTo(() => _fileService.DownloadBlobAsync(A<string>.That.IsEqualTo(relative_url))).Returns(new MemoryStream(Resources.Files.validzipfile));

            var downloadInfo = await _SUT.GetReleaseAsync(packageId, version);

            downloadInfo.FileContents.ShouldNotBe(null);
            downloadInfo.Filename.ShouldBe(relative_url);
            downloadInfo.MimeType.ShouldBe("application/zip");
        }

        [Theory]
        [InlineData("test.package", "")]
        [InlineData("unknown", "1.0.0.0")]
        public Task ThrowsExceptionOnInvalidIdOrVersion(string id, string version)
        {
            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored, A<ClaimsPrincipal>.Ignored)).Returns(default(Models.Release));

            return Should.ThrowAsync<InvalidDataException>(() => _SUT.GetReleaseAsync(id, version));
        }


        [Fact(Skip = "Requires a valid nuget package with known values.")]
        public void SyndicatesAPackage()
        {

        }


        [Fact]
        public async Task CanDeleteAPackage()
        {
            var relativeUri = "somedumburi";
            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored, A<ClaimsPrincipal>.Ignored)).Returns(new Models.Release { Id = "test.package", Version = "1.0.0.0", RelativeUri = relativeUri });
            A.CallTo(() => _releaseStore.DeleteAsync(A<string>.Ignored, A<string>.Ignored)).Returns(Task.FromResult(0));
            A.CallTo(() => _fileService.DeleteAsync(A<string>.That.IsEqualTo(relativeUri))).Returns(true);

            await _SUT.DeleteAsync("test.package", "1.0.0.0");
        }

        [Theory]
        [InlineData("bad.package", "0.0.0.0")]
        [InlineData("test.package", "0.0.0.0")]
        public Task ShouldThrow_FileNotFoundException_WithBad(string id, string version)
        {
            A.CallTo(() => _releaseStore.GetAsync(A<string>.That.IsEqualTo(id), A<string>.That.IsEqualTo(version), A<ClaimsPrincipal>.Ignored)).Returns(default(Models.Release));

            return Should.ThrowAsync<FileNotFoundException>(() => _SUT.DeleteAsync(id, version));
        }
    }
}
