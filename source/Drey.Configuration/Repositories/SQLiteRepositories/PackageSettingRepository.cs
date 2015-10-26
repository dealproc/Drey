using Dapper;

using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class PackageSettingRepository : SqlRepository, IPackageSettingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageSettingRepository"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public PackageSettingRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.PackageSetting> All()
        {
            return Execute(cn => cn.Query<DataModel.PackageSetting>("SELECT * FROM PackageSettings"));
        }

        /// <summary>
        /// Alls the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public IEnumerable<DataModel.PackageSetting> All(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.PackageSetting>("SELECT * FROM PackageSettings WHERE PackageId = @packageId", new { packageId = packageId }));
        }

        /// <summary>
        /// Bies the key.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string ByKey(string packageId, string key)
        {
            return Execute(cn => cn.ExecuteScalar<string>("SELECT Value FROM PackageSettings WHERE PackageId=@packageId AND Key = @key;", new { packageId = packageId, key = key }));
        }

        /// <summary>
        /// Gets the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public DataModel.PackageSetting Get(string packageId, string key)
        {
            return Execute(cn => cn.Query<DataModel.PackageSetting>("SELECT * FROM PackageSettings WHERE PackageId = @id AND Key = @key;", new { id = packageId, key = key }).FirstOrDefault());
        }

        /// <summary>
        /// Stores the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Store(Services.ViewModels.AppSettingPmo model)
        {
            Execute(cn =>
            {
                var parms = new { id = model.Id, packageId = model.PackageId, key = model.Key, Value = model.Value, createdOn = DateTime.Now, updatedOn = DateTime.Now };

                if (model.Id == 0) // assume insert
                {
                    cn.Execute(@"INSERT INTO PackageSettings (PackageId, Key, Value, CreatedOn, UpdatedOn) VALUES (@packageId, @key, @value, @createdOn, @updatedOn);", parms);
                    return;
                }

                cn.Execute(@"UPDATE PackageSettings SET PackageId = @packageId, Key = @key, Value = @value, UpdatedOn = @updatedOn, WHERE ID = @id;", parms);
            });
        }

        public void Store(DataModel.PackageSetting model)
        {
            Execute(cn =>
            {
                var parms = new { id = model.Id, packageId = model.PackageId, key = model.Key, Value = model.Value, createdOn = DateTime.Now, updatedOn = DateTime.Now };
                
                if (model.Id == 0) // assume insert.
                {
                    cn.Execute(@"INSERT INTO PackageSettings (PackageId, Key, Value, CreatedOn, UpdatedOn) VALUES(@packageId, @key, @value, @createdOn, @updatedOn);", parms);
                    return;
                }

                cn.Execute(@"UPDATE PackageSettings SET PackageId = @packageId, Key = @key, Value = @value, UpdatedOn = @updatedOn, WHERE ID = @id;", parms);
            });
        }
    }
}