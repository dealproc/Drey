using Dapper;
using Drey.Nut;
using System.Collections.Generic;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class PackageSettingRepository : SqlRepository, IPackageSettingRepository
    {
        public PackageSettingRepository(INutConfiguration configurationManager) : base(configurationManager) { }
        public IEnumerable<DataModel.PackageSetting> All()
        {
            return Execute(cn => cn.Query<DataModel.PackageSetting>("SELECT * FROM PackageSettings"));
        }

        public IEnumerable<DataModel.PackageSetting> All(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.PackageSetting>("SELECT * FROM PackageSettings WHERE PackageId = @packageId", new { packageId = packageId }));
        }

        public string ByKey(string packageId, string key)
        {
            return Execute(cn => cn.ExecuteScalar<string>("SELECT Value FROM PackageSettings WHERE PackageId=@packageId AND Key = @key;", new { packageId = packageId, key = key }));
        }
    }
}