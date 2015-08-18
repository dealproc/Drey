using Dapper;
using Drey.Nut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class PackageRepository : SqlRepository, IPackageRepository
    {
        public PackageRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        public IEnumerable<DataModel.RegisteredPackage> GetRegisteredPackages()
        {
            return Execute(cn => cn.Query<DataModel.RegisteredPackage>("SELECT * FROM [RegisteredPackages]"));
        }

        public DataModel.RegisteredPackage GetPackage(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.RegisteredPackage>("SELECT * FROM [RegisteredPackage] WHERE PackageId = @packageId", new { packageId = packageId }).SingleOrDefault());
        }

        public void Store(DataModel.RegisteredPackage package)
        {
            if (GetPackage(package.PackageId) == null)
            {
                Execute(cn => cn.Execute("INSERT [RegisteredPackage] (PackageId, CreatedOn, UpdatedOn) VALUES(@packageId, @createdOn, @updatedOn", new { packageId = package.Id, createdOn = DateTime.Now, updatedOn = DateTime.Now }));
            }
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return Execute(cn =>
                cn.Query<DataModel.Release>("SELECT [Releases].* FROM RELEASES JOIN [RegisteredPackage] ON [Releases].PackageId = [RegisteredPackages].Id WHERE [RegisteredPackage].[PackageId] = @packageId", new { packageId = packageId })
            );
        }
    }
}