using System;
using System.IO;

namespace Drey.Configuration.Tests.Repositories.SqliteRepositories
{
    public abstract class RepositoryTests : IDisposable
    {
        string _baseDir = @"c:\temp\";
        bool _disposed = false;
        string _dbName = Guid.NewGuid().ToString() + ".db3";

        protected string FilePath { get { return Path.Combine(_baseDir, _dbName); } }

        public RepositoryTests()
        {
            if (!Directory.Exists(_baseDir))
            {
                Directory.CreateDirectory(_baseDir);
            }
            Configuration.Infrastructure.Schema.MigrationManager.Migrate(FilePath);
        }
        ~RepositoryTests()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
