using Dapper;

using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class PackageRepository : SqlRepository, IPackageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageRepository"/> class.
        /// <remarks>Used by the IoC container.</remarks>
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public PackageRepository(INutConfiguration configurationManager) : base(configurationManager) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageRepository"/> class.
        /// <remarks>Used for integration testing.</remarks>
        /// </summary>
        /// <param name="databaseNameAndPath">The database name and path.</param>
        public PackageRepository(string databaseNameAndPath) : base(databaseNameAndPath) { }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.Release> All()
        {
            return Execute(cn => cn.Query<DataModel.Release>("SELECT * FROM Releases"))
                .Where(r => !r.Id.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.Package> GetPackages()
        {
            return Execute(cn => cn.Query<DataModel.Package>("SELECT Id, Min(Title) as Title, 1 as AutoUpdates FROM Releases GROUP BY Id COLLATE nocase")).Where(r => !r.Id.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return Execute(cn => cn.Query<DataModel.Release>(@"SELECT * FROM RELEASES WHERE Id COLLATE nocase = @packageId", new { packageId = packageId }));
        }

        /// <summary>
        /// Stores the specified release.
        /// </summary>
        /// <param name="release">The release.</param>
        /// <returns></returns>
        public DataModel.Release Store(DataModel.Release release)
        {
            return Execute(cn =>
            {
                var sqlParams = new
                {
                    id = release.Id,
                    version = release.Version,
                    description = release.Description,
                    iconUrl = release.IconUrl,
                    listed = release.Listed,
                    published = release.Published,
                    releaseNotes = release.ReleaseNotes,
                    summary = release.Summary,
                    tags = release.Tags,
                    title = release.Title,
                    sha1 = release.SHA1,
                    createdOn = DateTime.Now,
                    updatedOn = DateTime.Now
                };

                if (0 == cn.ExecuteScalar<int>("SELECT count(*) from Releases WHERE Id COLLATE nocase = @id AND Version COLLATE nocase = @version", sqlParams))
                {
                    cn.Execute(@"INSERT INTO Releases (Id, Version, Description, IconUrl, Listed, Published, ReleaseNotes, Summary, Tags, Title, SHA1, CreatedOn, UpdatedOn) 
VALUES (@id, @version, @description, @iconUrl, @listed, @published, @releaseNotes, @summary, @tags, @title, @sha1, @createdOn, @updatedOn);", sqlParams);
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
    UpdatedOn = @updatedOn
WHERE Id COLLATE nocase = @id AND Version = @version;
", sqlParams);
                }
                return release;
            });
        }

        /// <summary>
        /// Deletes all package references from the underlying store, matching the provided id and version.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="version">The version.</param>
        public void Delete(string packageId, string version)
        {
            Execute(cn => cn.Execute("DELETE FROM Releases WHERE Id COLLATE nocase = @packageId AND Version COLLATE nocase = @version", new { packageId, version }));
        }
    }
}