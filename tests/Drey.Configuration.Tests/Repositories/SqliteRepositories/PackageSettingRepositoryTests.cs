using Drey.Configuration.Repositories.SQLiteRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Drey.Configuration.Tests.Repositories.SqliteRepositories
{
    public class PackageSettingRepositoryTests : RepositoryTests
    {
        PackageSettingRepository _sut;

        public PackageSettingRepositoryTests()
        {
            _sut = new PackageSettingRepository(this.FilePath);
        }

        [Fact(DisplayName = "All Succeeds without data")]
        public void AllSucceeds_WithNoData()
        {
            _sut.All().Count().ShouldBe(0);
        }

        [Fact(DisplayName = "Can store a viewmodel")]
        public void CanStoreAViewModel()
        {
            var setting1 = new Configuration.Services.ViewModels.AppSettingPmo { Id = 0, PackageId = "pkg1", Key = "key1", Value = Guid.NewGuid().ToString() };
            _sut.Store(setting1);
            _sut.ByKey(setting1.PackageId, setting1.Key).ShouldBe(setting1.Value);
        }

        [Fact(DisplayName = "Can Update data")]
        public void CanUpdateData()
        {
            var setting1 = new Configuration.Services.ViewModels.AppSettingPmo { Id = 0, PackageId = "pkg1", Key = "key1", Value = Guid.NewGuid().ToString() };
            _sut.Store(setting1);

            var dbSetting = _sut.Get(setting1.PackageId, setting1.Key);
            dbSetting.Value = Guid.NewGuid().ToString();
            _sut.Store(dbSetting);

            _sut.ByKey(setting1.PackageId, setting1.Key).ShouldBe(dbSetting.Value);
        }


        [Fact(DisplayName = "Can retrieve a list of package settings for a package")]
        public void CanGetAllSettings_ForAPackage()
        {
            var allSettings = new DataModel.PackageSetting[]{
                new DataModel.PackageSetting { PackageId = "one", Key = "key.one", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "one", Key = "key.two", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "one", Key = "key.three", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.one", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.two", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.three", Value = Guid.NewGuid().ToString() },
            };

            allSettings.Apply(_sut.Store);

            _sut.All("one").Count().ShouldBe(3);
            _sut.All().Count().ShouldBe(allSettings.Length);
        }

        [Fact(DisplayName = "Can retrieve an entity by the package id and key")]
        public void CanGetADataModelByPackageIdAndKey()
        {
            var allSettings = new DataModel.PackageSetting[]{
                new DataModel.PackageSetting { PackageId = "one", Key = "key.one", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "one", Key = "key.two", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "one", Key = "key.three", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.one", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.two", Value = Guid.NewGuid().ToString() },
                new DataModel.PackageSetting { PackageId = "two", Key = "key.three", Value = Guid.NewGuid().ToString() },
            };

            allSettings.Apply(_sut.Store);

            Random rnd =new Random(allSettings.Length);
            var idx = rnd.Next(allSettings.Length);
            var expected = allSettings[idx];
            var test = _sut.Get(expected.PackageId, expected.Key);

            test.PackageId.ShouldBe(expected.PackageId);
            test.Key.ShouldBe(expected.Key);
            test.Value.ShouldBe(expected.Value);
        }
    }
}
