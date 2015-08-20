using Nancy;
using Nancy.Testing;

using Shouldly;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xunit;

namespace Drey.Server.Tests
{
    [Collection("Package Management")]
    public class ReleasesModuleTests : NancyTestingBase
    {
        [Theory]
        [InlineData("test.package", HttpStatusCode.OK)]
        [InlineData("unknown.package", HttpStatusCode.NotFound)]
        public void Should_Return_A_List_Of_Packages(string packageId, HttpStatusCode expectedResponse)
        {
            var result = TestBrowser.Get(".well-known/releases/" + packageId, with =>
            {
                with.HttpRequest();
            });

            Assert.Equal(expectedResponse, result.StatusCode);
            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                var response = result.Body.AsString();
                var releases = result.Body.DeserializeJson<IEnumerable<Models.Release>>();
                releases.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void WhenAnUnknownExceptionIsHandled_ServerShouldReturnA_5xx_Response()
        {
            var result = TestBrowser.Get(".well-known/releases/exception", with => with.HttpRequest());
            
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        }

        [Theory]
        [InlineData("test.package", "1.0.0.0", HttpStatusCode.OK)]
        [InlineData("test.package", "1.0.0.1", HttpStatusCode.NotFound)]
        [InlineData("unknown", "1.0.0.0", HttpStatusCode.NotFound)]
        [InlineData("exception", "1.0.0.0", HttpStatusCode.InternalServerError)]
        public void GettingAPackage_Tests(string id, string version, HttpStatusCode response)
        {
            var result = TestBrowser.Get(string.Format(".well-known/releases/{0}/{1}", id, version), with => with.HttpRequest());
            result.StatusCode.ShouldBe(response);
        }
    }
}