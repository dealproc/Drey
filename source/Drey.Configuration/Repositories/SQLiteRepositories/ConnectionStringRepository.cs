using Dapper;

using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class ConnectionStringRepository : SqlRepository, IConnectionStringRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringRepository"/> class.
        /// <remarks>Used by IoC container.</remarks>
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public ConnectionStringRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringRepository"/> class.
        /// <remarks>Used for integration testing.</remarks>
        /// </summary>
        /// <param name="databaseNameAndPath">The database name and path.</param>
        public ConnectionStringRepository(string databaseNameAndPath) : base(databaseNameAndPath) { }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.PackageConnectionString> All()
        {
            return Execute(cn => cn.Query<DataModel.PackageConnectionString>("SELECT * FROM ConnectionStrings"));
        }

        /// <summary>
        /// Alls the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public IEnumerable<DataModel.PackageConnectionString> All(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.PackageConnectionString>("SELECT * FROM ConnectionStrings WHERE PackageId COLLATE nocase = @packageId", new { packageId = packageId }));
        }

        /// <summary>
        /// Bies the name.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string ByName(string packageId, string name)
        {
            return Execute(cn => cn.ExecuteScalar<string>("SELECT ConnectionString FROM ConnectionStrings where PackageId COLLATE nocase = @packageId and Name COLLATE nocase = @name;", new { packageId = packageId, name = name }));
        }

        /// <summary>
        /// Gets the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public DataModel.PackageConnectionString Get(string packageId, string name)
        {
            return Execute(cn => cn.Query<DataModel.PackageConnectionString>("SELECT * FROM ConnectionStrings WHERE PackageId COLLATE nocase = @id and Name COLLATE nocase = @name;", new { id = packageId, name = name }).SingleOrDefault());
        }

        /// <summary>
        /// Stores the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Store(Services.ViewModels.ConnectionStringPmo model)
        {
            var parms = new
            {
                id = model.Id,
                packageId = model.PackageId,
                name = model.Name,
                connectionString = model.ConnectionString,
                providerName = model.ProviderName,
                createdOn = DateTime.Now,
                updatedOn = DateTime.Now
            };

            if (model.Id == 0)
            {
                Execute(cn =>
                {
                    if (Get(model.PackageId, model.Name) != null) { throw new UniqueIndexException("Violation of unique index."); }
                    cn.Execute(@"INSERT INTO ConnectionStrings (PackageId, Name, ConnectionString, ProviderName, CreatedOn, UpdatedOn) VALUES (@packageId, @name, @connectionString, @providerName, @createdOn, @updatedOn);", parms);
                });
                return;
            }

            Execute(cn => cn.Execute(@"UPDATE ConnectionStrings SET PackageId = @packageId, Name = @name, ConnectionString = @connectionString, ProviderName = @providerName, UpdatedOn = @updatedOn WHERE Id = @id", parms));
        }

        public void Store(DataModel.PackageConnectionString model)
        {
            var parms = new
            {
                id = model.Id,
                packageId = model.PackageId,
                name = model.Name,
                connectionString = model.ConnectionString,
                providerName = model.ProviderName,
                createdOn = DateTime.Now,
                updatedOn = DateTime.Now
            };

            if (model.Id == 0)
            {
                Execute(cn =>
                {
                    if (Get(model.PackageId, model.Name) != null) { throw new UniqueIndexException("Violation of unique index."); }
                    cn.Execute(@"INSERT INTO ConnectionStrings (PackageId, Name, ConnectionString, ProviderName, CreatedOn, UpdatedOn) VALUES (@packageId, @name, @connectionString, @providerName, @createdOn, @updatedOn);", parms);
                });
                return;
            }

            Execute(cn => cn.Execute(@"UPDATE ConnectionStrings SET PackageId = @packageId, Name = @name, ConnectionString = @connectionString, ProviderName = @providerName, UpdatedOn = @updatedOn WHERE Id = @id", parms));
        }

        /// <summary>
        /// Deletes the package connection string, by its key.
        /// </summary>
        /// <param name="id">The key for the connection string to be deleted.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Delete(int id)
        {
            Execute(cn => cn.Execute("DELETE FROM ConnectionStrings WHERE Id=@id", new { id }));
        }
    }
}