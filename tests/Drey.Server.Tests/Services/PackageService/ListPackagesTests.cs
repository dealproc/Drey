using FakeItEasy;
using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class ListPackagesTests : PackageServiceTests
    {
        [Fact]
        public void ListPackagesShouldCallStore()
        {
            _SUT.ListPackages();

            A.CallTo(() => _packageStore.Packages()).MustHaveHappened();
        }
    }
}