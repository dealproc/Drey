using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Drey.Configuration.Repositories;
using FakeItEasy;
using Drey.Configuration.Services;

namespace Drey.Configuration.Tests.Services
{
    public class PackageServiceTests
    {
        IPackageRepository _packageRepository;
        IPackageService _SUT;
        string _packageId = "some.package";

        public PackageServiceTests()
        {
            _packageRepository = A.Fake<IPackageRepository>(opts =>
            {
                opts.Strict();
            });
            A.CallTo(() => _packageRepository.GetPackages()).Returns(new[] 
            {
                new DataModel.Package { Id = _packageId, Title = "Some Package" },
                new DataModel.Package { Id = "second.package", Title = "Second Package" }
            });


            _SUT = new PackageService(_packageRepository, A.Dummy<IConnectionStringRepository>(), A.Dummy<IPackageSettingRepository>());
        }

        [Fact]
        public void CanGetReleases()
        {
            A.CallTo(() => _packageRepository.GetReleases(A<string>.That.IsEqualTo(_packageId))).Returns(new[]
            {
                new DataModel.Release(),
                new DataModel.Release()
            });

            var rels = _SUT.GetReleases(_packageId);

            rels.ShouldNotBe(null);
            rels.Any().ShouldBe(true);
        }

        [Fact]
        public void CanGetPackages()
        {
            var pkgs = _SUT.GetPackages();

            pkgs.ShouldNotBe(null);
            pkgs.Any().ShouldBe(true);
        }

        [Fact]
        public void CanDiffReleases()
        {
            A.CallTo(() => _packageRepository.GetReleases(A<string>.That.IsEqualTo(_packageId)))
                .Returns(new[]
                {
                    new DataModel.Release { Id = _packageId, Version = "1.0.0.0", SHA1 = "SHA1" }
                });
            var newReleases = new[] 
            { 
                new DataModel.Release { Id = _packageId, Version = "1.0.0.0", SHA1 = "SHA1" },
                new DataModel.Release { Id = _packageId, Version = "1.0.0.1", SHA1 = "SHA2" } 
            };

            var delta = _SUT.Diff(_packageId, newReleases);

            delta.ShouldNotBe(null);
            delta.Count().ShouldBe(1);
            delta.ShouldContain(x => x.Id == _packageId && x.Version == "1.0.0.1" && x.SHA1 == "SHA2");
        }

        [Fact]
        public void ReleasesWithUnregisteredPackageId_ShouldDiff_AllReleases()
        {
            A.CallTo(() => _packageRepository.GetReleases(A<string>.Ignored))
                .Returns(Enumerable.Empty<DataModel.Release>());

            var newReleases = new[]
            {
                new DataModel.Release { Id = "new.package", Version = "1.0.0.0", SHA1 = "SHA1"},
                new DataModel.Release { Id = "new.package", Version = "1.1.0.0", SHA1 = "SHA2"},
                new DataModel.Release { Id = "new.package", Version = "1.2.0.0", SHA1 = "SHA3"},
                new DataModel.Release { Id = "new.package", Version = "1.3.0.0", SHA1 = "SHA4"},
            };

            var delta = _SUT.Diff("new.package", newReleases);

            delta.ShouldAllBe(x => x.Id == "new.package");
            delta.Count().ShouldBe(4);
        }

        [Fact]
        public void CanRecordReleases()
        {
            A.CallTo(() => _packageRepository.Store(A<DataModel.Release>.Ignored)).Returns(default(DataModel.Release));

            var newReleases = new[]
            {
                new DataModel.Release { Id = "new.package", Version = "1.0.0.0", SHA1 = "SHA1"},
                new DataModel.Release { Id = "new.package", Version = "1.1.0.0", SHA1 = "SHA2"},
                new DataModel.Release { Id = "new.package", Version = "1.2.0.0", SHA1 = "SHA3"},
                new DataModel.Release { Id = "new.package", Version = "1.3.0.0", SHA1 = "SHA4"},
            };

            _SUT.RecordReleases(newReleases);

            A.CallTo(() => _packageRepository.Store(A<DataModel.Release>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }
    }
}