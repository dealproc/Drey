using Nancy;
using Shouldly;
using Xunit;

namespace Drey.Server.Tests.Modules
{
    public class NugetUploadModuleTests : NancyTestingBase
    {
        [Fact]
        public void ANugetPackageCanBeDeleted()
        {
            var result = TestBrowser.Delete("/api/v2/package/test.package/1.0.0.0", with => with.HttpRequest());

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}