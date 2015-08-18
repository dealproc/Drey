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
            return Execute(cn => cn.Query<DataModel.RegisteredPackage>("SELECT * FROM RegisteredPackages"));
        }

        public DataModel.RegisteredPackage GetPackage(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.RegisteredPackage>("SELECT * FROM RegisteredPackages WHERE PackageId = @packageId", new { packageId = packageId }).SingleOrDefault());
        }

        public void Store(DataModel.RegisteredPackage package)
        {
            if (GetPackage(package.PackageId) == null)
            {
                Execute(cn =>
                {
                    var id = cn.ExecuteScalar<int>(@"INSERT INTO RegisteredPackages (PackageId, CreatedOn, UpdatedOn) 
VALUES(@packageId, @createdOn, @updatedOn); 
select last_insert_rowid();",
                        new { packageId = package.PackageId, createdOn = DateTime.Now, updatedOn = DateTime.Now });
                    package.Id = id;
                    return package;
                });
            }
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return Execute(cn =>
                cn.Query<DataModel.Release>(@"SELECT Releases.* FROM RELEASES JOIN RegisteredPackages ON Releases.RegisteredPackageId = RegisteredPackages.Id WHERE RegisteredPackages.PackageId = @packageId", new { packageId = packageId })
            );
        }

        public DataModel.Release Store(DataModel.Release release)
        {
            if (release.Package == null) { throw new NullReferenceException("Package has not been assigned to this release."); }

            return Execute(cn =>
            {
                var id = cn.ExecuteScalar<int>(@"INSERT INTO Releases (RegisteredPackageId, SHA1, Filename, Ordinal, CreatedOn, UpdatedOn) 
VALUES (@registeredPackageId, @sha1, @fileName, @ordinal, @createdOn, @updatedOn);
select last_insert_rowid();",
                    new { registeredPackageId = release.Package.Id, sha1 = release.SHA1, fileName = release.Filename, ordinal = release.Ordinal, createdOn = DateTime.Now, updatedOn = DateTime.Now });
                release.Id = id;
                return release;
            });
        }
    }
}