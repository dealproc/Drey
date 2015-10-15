using Nancy;
using Shouldly;
using Xunit;

namespace Drey.Server.Tests.Modules
{
    public class NugetUploadModuleTests : IClassFixture<ApiTestFixture>
    {
        ApiTestFixture _testFixture;

        public NugetUploadModuleTests(ApiTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public void ANugetPackageCanBeDeleted()
        {
            var result = _testFixture.TestBrowser.Delete("/api/v2/package/test.package/1.0.0.0", with => with.HttpRequest());

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact(Skip = "Inquire/discuss with Nancy team on how to properly integrate-test an upload")]
        public void CanSyndicateAPackage()
        {

        }
    }
}