using Drey.Server.Services;

using FakeItEasy;

using Shouldly;

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Services
{
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
            A.CallTo(() => _releaseStore.ListPackages()).Returns(new[] { new Models.Package { } });

            var listOfPackages = await _SUT.GetPackagesAsync();

            listOfPackages.ShouldNotBe(null);
            listOfPackages.Count().ShouldBe(1);

            A.CallTo(() => _releaseStore.ListPackages()).MustHaveHappened();
        }

        [Fact]
        public async Task CanQueryReleasesAsync()
        {
            string test_package_id = "test.package";
            A.CallTo(() => _releaseStore.ListByIdAsync(A<string>.That.IsEqualTo(test_package_id)))
                .Returns(
                new[]
                {
                    new Models.Release{ Id = test_package_id, Version = "1.0.0.0" },
                    new Models.Release{ Id = test_package_id, Version = "1.1.0.0" },
                    new Models.Release{ Id = test_package_id, Version = "1.2.0.0" },
                });

            var releases = await _SUT.GetReleasesAsync(test_package_id);

            A.CallTo(() => _releaseStore.ListByIdAsync(A<string>.That.IsEqualTo(test_package_id))).MustHaveHappened();
            releases.ShouldNotBe(null);
            releases.Count().ShouldBe(3);
        }

        [Fact]
        public async Task CanRetrieveAFileFromStorageAsync()
        {
            string packageId = "test.package";
            string version = "1.0.0.0";
            string relative_url = string.Format("{0}-{1}.nupkg", packageId, version);

            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored)).Returns(new Models.Release { Id = packageId, Version = version, RelativeUri = relative_url });
            A.CallTo(() => _fileService.DownloadBlobAsync(A<string>.That.IsEqualTo(relative_url))).Returns(new MemoryStream(Resources.Files.validzipfile));

            var downloadInfo = await _SUT.GetReleaseAsync(packageId, version);

            downloadInfo.FileContents.ShouldNotBe(null);
            downloadInfo.Filename.ShouldBe(relative_url);
            downloadInfo.MimeType.ShouldBe("application/zip");
        }

        [Theory]
        [InlineData("test.package", "")]
        [InlineData("unknown", "1.0.0.0")]
        public void ThrowsExceptionOnInvalidIdOrVersion(string id, string version)
        {
            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored)).Returns(default(Models.Release));

            Should.Throw<InvalidDataException>(async () => await _SUT.GetReleaseAsync(id, version));
        }


        [Fact]
        public async Task CanDeleteAPackage()
        {
            var relativeUri = "somedumburi";
            A.CallTo(() => _releaseStore.GetAsync(A<string>.Ignored, A<string>.Ignored)).Returns(new Models.Release { Id = "test.package", Version = "1.0.0.0", RelativeUri = relativeUri });
            A.CallTo(() => _releaseStore.DeleteAsync(A<string>.Ignored, A<string>.Ignored)).Returns(Task.FromResult(0));
            A.CallTo(() => _fileService.DeleteAsync(A<string>.That.IsEqualTo(relativeUri))).Returns(true);

            await _SUT.DeleteAsync("test.package", "1.0.0.0");
        }

        [Theory]
        [InlineData("bad.package", "0.0.0.0")]
        [InlineData("test.package", "0.0.0.0")]
        public void ShouldThrow_FileNotFoundException_WithBad(string id, string version)
        {
            A.CallTo(() => _releaseStore.GetAsync(A<string>.That.IsEqualTo(id), A<string>.That.IsEqualTo(version))).Returns(default(Models.Release));

            Should.Throw<FileNotFoundException>(async () => await _SUT.DeleteAsync(id, version));
        }
    }
}
