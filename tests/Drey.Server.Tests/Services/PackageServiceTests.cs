using Drey.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;
using Shouldly;
using System.Security.Cryptography;
using System.IO;

namespace Drey.Server.Tests.Services
{
    public class PackageServiceTests
    {
        protected IPackageStore _packageStore;
        protected IFileService _fileService;
        protected string _knownSHA;
        protected string _badSHA;

        protected IPackageService _SUT;

        public PackageServiceTests()
        {
            _packageStore = A.Fake<IPackageStore>((opts) =>
            {
                opts.Strict();
            });
            _fileService = A.Fake<IFileService>((opts) =>
            {
                opts.Strict();
            });

            using (var sha = SHA1Managed.Create())
            {
                _knownSHA = BitConverter.ToString(sha.ComputeHash(Guid.NewGuid().ToByteArray())).Replace("-", string.Empty).ToUpper();
                _badSHA = BitConverter.ToString(sha.ComputeHash(Guid.NewGuid().ToByteArray())).Replace("-", string.Empty).ToUpper();
            }

            A.CallTo(() => _packageStore.Packages())
                .Returns(new[] { 
                        new Models.Package { 
                            PackageId = "good.package", 
                            Releases = new List<Models.Release> { 
                                new Models.Release { 
                                    Filename = "goodfilename.zip", 
                                    Filesize = 1024, 
                                    SHA1 = _knownSHA 
                                } 
                            } 
                        }
                    });

            _SUT = new Drey.Server.Services.PackageService(_packageStore, _fileService);
        }
    }

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
                    Assert.Throws<ArgumentException>(() => _SUT.CreateRelease(packageId, fileName, fileStream));
                    break;
                case ExceptionTypes.argumentNull:
                    Assert.Throws<ArgumentNullException>(() => _SUT.CreateRelease(packageId, fileName, fileStream));
                    break;
            }
        }
    }

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
            Assert.Throws<KeyNotFoundException>(() =>
            {
                _SUT.DeleteRelease(_badSHA);
            });
        }
    }
}