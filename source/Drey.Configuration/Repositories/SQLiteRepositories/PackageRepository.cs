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

        public IEnumerable<DataModel.Package> GetPackages()
        {
            return Execute(cn => cn.Query<DataModel.Package>("SELECT Id, Min(Title) as Title FROM Releases GROUP BY Id"));
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.Release>(@"SELECT * FROM RELEASES WHERE Id = @packageId", new { packageId = packageId }));
        }

        public DataModel.Release Store(DataModel.Release release)
        {
            return Execute(cn =>
            {
                var sqlParams = new { registeredPackageId = release.Id, sha1 = release.SHA1, fileName = release.Filename, createdOn = DateTime.Now, updatedOn = DateTime.Now };

                if (0 == cn.ExecuteScalar<int>("SELECT count(*) from Releases WHERE Id = @id AND Version = @version", sqlParams))
                {
                    cn.Execute(@"INSERT INTO Releases (Id, Version, Description, IconUrl, Listed, Published, ReleaseNotes, Summary, Tags, Title, SHA1, Filename, CreatedOn, UpdatedOn) 
VALUES (@id, @version, @description, @iconUrl, @listed, @published, @releaseNotes, @summary, @tags, @title, @sha1, @filename, @createdOn, @updatedOn);", sqlParams);
                }
                else
                {
                    cn.Execute(
@"UPDATE Releases SET 
    Description = @description,
    IconUrl = @iconUrl,
    Listed = @listed,
    Published = @published,
    ReleaseNotes = @releaseNotes,
    Summary = @summary,
    Tags = @tags,
    Title = @title,
    SHA1 = @sha1,
    Filename = @filename,
    UpdatedOn = @updatedOn
WHERE Id = @id AND Version = @version;
", sqlParams);
                }
                return release;
            });
        }
    }
}