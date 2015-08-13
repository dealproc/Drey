using Nancy;
using Nancy.Testing;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Drey.Server.Tests
{
    [Collection("Package Management")]
    public class PackagesModuleTests : NancyTestingBase
    {
        [Fact]
        public void Can_List_Packages()
        {
            var result = TestBrowser.Get(".well-known/packages", with =>
            {
                with.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var packages = result.Body.DeserializeJson<IEnumerable<Models.Package>>();
            Assert.True(packages.Any());
        }
    }
}
