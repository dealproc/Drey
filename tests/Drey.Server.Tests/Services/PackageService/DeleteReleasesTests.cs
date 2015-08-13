using FakeItEasy;

using Shouldly;

using System.Collections.Generic;

using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class DeleteReleasesTests : PackageServiceTests
    {
        [Fact]
        public void AKnownReleaseShouldBeDeletable()
        {
            A.CallTo(() => _fileService.DeleteAsync(A<string>.Ignored)).Returns(true);
            A.CallTo(() => _packageStore.DeleteRelease(A<string>.Ignored)).DoesNothing();

            _SUT.DeleteRelease(_knownSHA);

            A.CallTo(() => _packageStore.Packages()).MustHaveHappened();
            A.CallTo(() => _packageStore.DeleteRelease(A<string>.That.IsEqualTo(_knownSHA))).MustHaveHappened();
            A.CallTo(() => _fileService.DeleteAsync(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void AnUknownSHAShouldThowKeyNotFoundException()
        {
            Should.Throw<KeyNotFoundException>(() => _SUT.DeleteRelease(_badSHA));
        }
    }
}