using Nancy;
using Nancy.Testing;
using System.IO;
using System.Linq;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using System;

namespace Drey.Server.Tests
{
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


        public static IEnumerable<object[]> Packages
        {
            get
            {
                return new[]
                {
                    new object[] {"empty.package", "empty-program.1.0.0.0.zip", "octet/stream", new byte[0], HttpStatusCode.BadRequest },
                    new object[] {"good.package", "good-program.1.0.0.0.zip", "octet/stream", Resources.Files.validzipfile, HttpStatusCode.Created },
                };
            }
        }
        [Theory]
        [MemberData("Packages")]
        public void Should_Create_A_Package(string packageId, string fileName, string mimeType, byte[] fileStream, HttpStatusCode expectedStatusCode)
        {
            var ms = new MemoryStream(fileStream);
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