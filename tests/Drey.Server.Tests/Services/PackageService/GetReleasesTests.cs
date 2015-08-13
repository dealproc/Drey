using FakeItEasy;
using Shouldly;
using System.Linq;
using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class GetReleasesTests : PackageServiceTests
    {
        public GetReleasesTests()
        {
            A.CallTo(() => _packageStore.Releases(A<string>.Ignored)).Returns(null);
            A.CallTo(() => _packageStore.Releases(A<string>.That.IsEqualTo(_goodPackageId)))
                .Returns(_packageStore.Packages().Where(p => p.PackageId == _goodPackageId).Single().Releases);
        }

        [Fact]
        public void ReleasesShouldBeReturnedForAKnownPackage()
        {
            var releases = _SUT.GetReleases(_goodPackageId);

            releases.ShouldNotBe(null);
            releases.Count().ShouldBe(1);
        }

        [Fact]
        public void NothingShouldBeReturnedForAnUnknownPackageId()
        {
            _SUT.GetReleases("badpackage").ShouldBe(null);
        }
    }
}