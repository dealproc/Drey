using FakeItEasy;

using Shouldly;

using System.Collections.Generic;
using System.IO;

using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class GetReleaseTests : PackageServiceTests
    {
        [Fact]
        public void AGoodSHAShouldAllowRetrievalOfItsFile()
        {
            A.CallTo(() => _fileService.DownloadBlobAsync(A<string>.Ignored)).Returns(new MemoryStream(new byte[10]));

            var file = _SUT.GetRelease(_knownSHA);

            file.Filename.ShouldNotBeEmpty();
            file.MimeType.ShouldNotBeEmpty();
            file.MimeType.ShouldBe("octet/stream");
            file.FileContents.ShouldNotBe(null);
        }

        [Fact]
        public void AnUnknownSHAShouldThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => _SUT.GetRelease(_badSHA));
        }
    }
}