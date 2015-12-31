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
    public class ConnectionStringRepositoryTests : RepositoryTests
    {
        ConnectionStringRepository _sut;

        public ConnectionStringRepositoryTests()
            : base()
        {
            _sut = new ConnectionStringRepository(this.FilePath);
        }

        [Fact(DisplayName = "Can query with no data, and get no results back successfully.")]
        public void QueryWithNoDataSuccess()
        {
            _sut.All().Count().ShouldBe(0);
        }

        [Fact(DisplayName = "Can Store a viewmodel successfully.")]
        public void CanStoreAViewModel()
        {
            var vm = new Configuration.Services.ViewModels.ConnectionStringPmo { Name = "connection string name", ConnectionString = "connection string", PackageId = "package id", ProviderName = "provider name" };
            _sut.Store(vm);
            var test = _sut.Get(vm.PackageId, vm.Name);

            test.ConnectionString.ShouldBe(vm.ConnectionString);
            test.Name.ShouldBe(vm.Name);
            test.PackageId.ShouldBe(vm.PackageId);
            test.ProviderName.ShouldBe(vm.ProviderName);
        }

        [Fact(DisplayName = "Can store an entity successfully.")]
        public void CanStoreAnEntitySuccessfully()
        {
            var entity = new DataModel.PackageConnectionString { Name = "connection string name", ConnectionString = Guid.NewGuid().ToString(), PackageId = "package id", ProviderName = "provider name" };
            _sut.Store(entity);
            var test = _sut.Get(entity.PackageId, entity.Name);

            test.Id.ShouldNotBe(0);
            test.ConnectionString.ShouldBe(entity.ConnectionString);
            test.Name.ShouldBe(entity.Name);
            test.PackageId.ShouldBe(entity.PackageId);
            test.ProviderName.ShouldBe(entity.ProviderName);
        }

        [Fact(DisplayName = "Can retrieve a connection string by its package id and name")]
        public void RetrievesConnectionStringByItsPackageIdAndName()
        {
            var entities = PopulateRepository();

            Random rand = new Random(entities.Length);
            var idx = rand.Next(entities.Length);
            var expected = entities[idx];

            _sut.ByName(expected.PackageId, expected.Name).ShouldBe(expected.ConnectionString);
        }

        [Fact(DisplayName = "Can Retrieve data when data exists within the database.")]
        public void CanRetrieveDataWhenInDatabase()
        {
            var entities = PopulateRepository();

            _sut.All().Count().ShouldBe(entities.Length);
        }

        [Fact(DisplayName = "Can retrieve data for a given package")]
        public void CanRetrieveDataForASinglePackage()
        {
            var entities = PopulateRepository();

            _sut.All("one").Count().ShouldBe(2);
        }

        [Fact(DisplayName = "Can get an entity from the database.")]
        public void CanGetAnEntityFromTheStore()
        {
            var entities = PopulateRepository();

            Random rand = new Random(entities.Length);
            var idx = rand.Next(entities.Length);
            var expect = entities[idx];

            var test = _sut.Get(expect.PackageId, expect.Name);

            test.Id.ShouldNotBe(0);
            test.ConnectionString.ShouldBe(expect.ConnectionString);
            test.Name.ShouldBe(expect.Name);
            test.PackageId.ShouldBe(expect.PackageId);
            test.ProviderName.ShouldBe(expect.ProviderName);
        }

        [Fact(DisplayName = "Can retrieve a connection string from the repository by its package id and name.")]
        public void CanGetAConnectionStringByPackageIdAndKey()
        {
            var entities = PopulateRepository();

            Random rand = new Random(entities.Length);
            var idx = rand.Next(entities.Length);
            var expect = entities[idx];

            var test = _sut.ByName(expect.PackageId, expect.Name);

            test.ShouldBe(expect.ConnectionString);
        }

        [Fact(DisplayName = "Can update a viewmodel that was created from an entity.")]
        public void CanUpdateFromAViewModel()
        {
            var entities = PopulateRepository();

            Random rand = new Random(entities.Length);
            var idx = rand.Next(entities.Length);
            var chosen = entities[idx];

            var entity = _sut.Get(chosen.PackageId, chosen.Name);

            var vm = new Configuration.Services.ViewModels.ConnectionStringPmo { Id = entity.Id, ConnectionString = entity.ConnectionString, Name = entity.Name, ProviderName = entity.ProviderName, PackageId = entity.PackageId };

            vm.Name = Guid.NewGuid().ToString();
            vm.PackageId = Guid.NewGuid().ToString();
            vm.ProviderName = Guid.NewGuid().ToString();
            vm.ConnectionString = Guid.NewGuid().ToString();

            _sut.Store(vm);

            var test = _sut.Get(vm.PackageId, vm.Name);

            test.ConnectionString.ShouldBe(vm.ConnectionString);
            test.PackageId.ShouldBe(vm.PackageId);
            test.ProviderName.ShouldBe(vm.ProviderName);
            test.Name.ShouldBe(vm.Name);
        }

        [Fact(DisplayName = "Can update an entity")]
        public void CanUpdateAnEntity()
        {
            var entities = PopulateRepository();

            Random rand = new Random(entities.Length);
            var idx = rand.Next(entities.Length);
            var chosen = entities[idx];

            var entity = _sut.Get(chosen.PackageId, chosen.Name);

            entity.Name = Guid.NewGuid().ToString();
            entity.PackageId = Guid.NewGuid().ToString();
            entity.ProviderName = Guid.NewGuid().ToString();
            entity.ConnectionString = Guid.NewGuid().ToString();

            _sut.Store(entity);

            var test = _sut.Get(entity.PackageId, entity.Name);

            test.ConnectionString.ShouldBe(entity.ConnectionString);
            test.PackageId.ShouldBe(entity.PackageId);
            test.ProviderName.ShouldBe(entity.ProviderName);
            test.Name.ShouldBe(entity.Name);
        }

        private DataModel.PackageConnectionString[] PopulateRepository()
        {
            var entities = new DataModel.PackageConnectionString[]{
                new DataModel.PackageConnectionString { PackageId = "one",    ProviderName = Guid.NewGuid().ToString(),  Name = "sql1", ConnectionString = Guid.NewGuid().ToString(), },
                new DataModel.PackageConnectionString { PackageId = "one",    ProviderName = Guid.NewGuid().ToString(),  Name = "sql2", ConnectionString = Guid.NewGuid().ToString(), },
                new DataModel.PackageConnectionString { PackageId = "three",  ProviderName = Guid.NewGuid().ToString(),  Name = "sql1", ConnectionString = Guid.NewGuid().ToString(), },
                new DataModel.PackageConnectionString { PackageId = "two",    ProviderName = Guid.NewGuid().ToString(),  Name = "sql1", ConnectionString = Guid.NewGuid().ToString(), },
            };

            entities.Apply(_sut.Store);
            return entities;
        }
    }
}
