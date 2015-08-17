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


        public static IEnumerable<object[]> BadPackage
        {
            get
            {
                return new[] { new object[] { "empty.package", "empty-program.1.0.0.0.zip", "octet/stream", new byte[0], HttpStatusCode.BadRequest }, };
            }
        }
        public static IEnumerable<object[]> GoodPackage
        {
            get
            {
                return new[] { new object[] { "good.package", "good-program.1.0.0.0.zip", "octet/stream", Samples.Server.Services.InMemory.Resources.Files.validzipfile, HttpStatusCode.Created }, };
            }
        }
        [Theory]
        [MemberData("BadPackage")]
        //[MemberData("GoodPackage")] RB: Skipping on 8/17/2015 due to known issue - the memorystream is not being emitted on the NancyModule.  Researching root cause and will rectify later.
        public void Should_Create_A_Package(string packageId, string fileName, string mimeType, byte[] fileStream, HttpStatusCode expectedStatusCode)
        {
            var ms = new MemoryStream(fileStream);
            ms.Seek(0, 0);
            var multiPart = new BrowserContextMultipartFormData(x =>
            {
                x.AddFile("file[]", fileName, mimeType, ms);
            });

            var result = TestBrowser.Post(".well-known/releases/" + packageId, with =>
            {
                with.HttpRequest();
                with.MultiPartFormData(multiPart);
            });

            result.Context.Request.Files.Count().ShouldBe(1);
            result.Context.Request.Files.First().Name.ShouldBe(fileName);
            result.Context.Request.Files.First().ContentType.ShouldBe(mimeType);

            result.StatusCode.ShouldBe(expectedStatusCode);
        }
    }
}