using Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests
{
    public class ReleasesModuleTests : IClassFixture<ApiTestFixture>
    {
        ApiTestFixture _testFixture;

        public ReleasesModuleTests(ApiTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Theory]
        [InlineData("test.package", HttpStatusCode.OK)]
        [InlineData("unknown.package", HttpStatusCode.NotFound)]
        public Task Should_Return_A_List_Of_Packages_async(string packageId, HttpStatusCode expectedResponse)
        {
            return _testFixture.WithOwinServer((client) =>
            {
                var response = client.GetAsync(".well-known/releases/" + packageId).Result;


                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    var result = response.Content.ReadAsAsync<IEnumerable<Models.Release>>().Result;
                    result.Count().ShouldBe(1);
                }
                //else
                //{
                //    var content = response.Content.ReadAsStringAsync().Result;
                //    Console.WriteLine(content);
                //}

                response.StatusCode.ShouldBe(expectedResponse);
                return Task.FromResult(0);
            });
        }

        [Fact]
        public async Task WhenAnUnknownExceptionIsHandled_ServerShouldReturnA_5xx_Response()
        {
            await _testFixture.WithOwinServer(async (client) =>
            {
                var response = await client.GetAsync(".well-known/releases/exception");
                response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            });
        }

        [Theory]
        [InlineData("test.package", "1.0.0.0", HttpStatusCode.OK)]
        [InlineData("test.package", "1.0.0.1", HttpStatusCode.NotFound)]
        [InlineData("unknown", "1.0.0.0", HttpStatusCode.NotFound)]
        [InlineData("exception", "1.0.0.0", HttpStatusCode.InternalServerError)]
        public Task GettingAPackage_Tests(string id, string version, HttpStatusCode response)
        {
            return _testFixture.WithOwinServer((client) =>
            {
                var webResponse = client.GetAsync(string.Format(".well-known/releases/{0}/{1}", id, version)).Result;
                webResponse.StatusCode.ShouldBe(response);
                return Task.FromResult(0);
            });
        }
    }
}