using Dapper;
using Drey.Nut;
using System.Collections.Generic;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class ConnectionStringRepository : SqlRepository, IConnectionStringRepository
    {
        public ConnectionStringRepository(INutConfiguration configurationManager) : base(configurationManager) { }
        public IEnumerable<DataModel.PackageConnectionString> All()
        {
            return Execute(cn => cn.Query<DataModel.PackageConnectionString>("SELECT * FROM ConnectionStrings"));
        }

        public IEnumerable<DataModel.PackageConnectionString> All(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.PackageConnectionString>("SELECT * FROM ConnectionStrings WHERE PackageId = @packageId", new { packageId = packageId }));
        }

        public string ByKey(string packageId, string key)
        {
            return Execute(cn => cn.ExecuteScalar<string>("SELECT ConnectionString FROM ConnectionStrings where PackageId = @packageId and Key = @key", new { packageId = packageId, key = key }));
        }
    }
}