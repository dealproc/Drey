using Drey.Server.Services;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Xunit;

namespace Drey.Server.Tests.Services
{
    [Collection("Package Service")]
    public class PackageServiceTests
    {
        protected IPackageStore _packageStore;
        protected IFileService _fileService;
        protected string _knownSHA;
        protected string _badSHA;
        protected string _goodPackageId = "good.package";
        protected string _goodFileName = "goodfilename.zip";

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
                            PackageId = _goodPackageId, 
                            Releases = new List<Models.Release> { 
                                new Models.Release { 
                                    Filename = _goodFileName, 
                                    Filesize = 1024, 
                                    SHA1 = _knownSHA 
                                } 
                            } 
                        }
                    });

            _SUT = new Drey.Server.Services.PackageService(_packageStore, _fileService);
        }
    }
}