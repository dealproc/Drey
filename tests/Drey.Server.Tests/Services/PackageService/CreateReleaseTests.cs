using FakeItEasy;

using Shouldly;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace Drey.Server.Tests.Services.PackageService
{
    public class CreateReleaseTests : PackageServiceTests
    {
        public enum ExceptionTypes
        {
            none,
            argument,
            argumentNull
        };

        public static IEnumerable<object[]> EmptyStream { get { return new[] { new object[] { "good.package", "good.file", new MemoryStream(), ExceptionTypes.argument } }; } }
        public static IEnumerable<object[]> GoodStream { get { return new[] { new object[] { "good.package", "good.file", new MemoryStream(new byte[10]), ExceptionTypes.none } }; } }

        [Theory]
        [InlineData(null, null, null, ExceptionTypes.argument)]
        [InlineData("good.package", null, null, ExceptionTypes.argument)]
        [InlineData("good.package", "", null, ExceptionTypes.argument)]
        [InlineData("good.package", "good.file", null, ExceptionTypes.argumentNull)]
        [MemberData("EmptyStream")]
        [MemberData("GoodStream")]
        public void ParameterExceptionTests(string packageId, string fileName, System.IO.Stream fileStream, ExceptionTypes expectedException)
        {
            A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(fileName), A<Stream>.Ignored)).Returns(Task.FromResult(fileName));
            A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).DoesNothing();

            switch (expectedException)
            {
                case ExceptionTypes.none:
                    _SUT.CreateRelease(packageId, fileName, fileStream).ShouldBe(true);
                    A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(fileName), A<Stream>.Ignored)).MustHaveHappened();
                    A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).MustHaveHappened();
                    break;
                case ExceptionTypes.argument:
                    Should.Throw<ArgumentException>(() => _SUT.CreateRelease(packageId, fileName, fileStream));
                    break;
                case ExceptionTypes.argumentNull:
                    Should.Throw<ArgumentNullException>(() => _SUT.CreateRelease(packageId, fileName, fileStream));
                    break;
            }
        }

        [Fact]
        public void APackgaeShouldBeCreatedWhenProvidedAnUnknownId()
        {
            string testFilename = "test.zip";
            string testPackage = "new.test.package";

            A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(testFilename), A<Stream>.Ignored)).Returns(Task.FromResult(testFilename));
            A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).DoesNothing();

            _SUT.CreateRelease(testPackage, testFilename, new MemoryStream(new byte[10])).ShouldBe(true);

            A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(testFilename), A<Stream>.Ignored)).MustHaveHappened();
            A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void AReleaseShouldBeAddedToAnExistingPackage()
        {
            string testFilename = "test.zip";
            string testPackage = "new.test.package";

            A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(testFilename), A<Stream>.Ignored)).Returns(Task.FromResult(testFilename));
            A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).DoesNothing();

            _SUT.CreateRelease(_goodPackageId, testFilename, new MemoryStream(new byte[10])).ShouldBe(true);

            A.CallTo(() => _fileService.StoreAsync(A<string>.That.IsEqualTo(testFilename), A<Stream>.Ignored)).MustHaveHappened();
            A.CallTo(() => _packageStore.Store(A<Models.Package>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}