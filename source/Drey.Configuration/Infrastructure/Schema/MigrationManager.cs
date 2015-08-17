using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using System;
using System.IO;
using System.Linq;

namespace Drey.Configuration.Infrastructure.Schema
{
    public static class MigrationManager
    {
        static int MAX_BACKUP_FILES = 5;
        static string CONFIG_FILE_NAME = "config.db3";
        static string DB_BACKUP_EXT = ".bak";
        static string DB_BACKUP_FILENAME_FORMAT = "config.{0:yyyy-MM-dd}-{0:HH-mm-ss}" + DB_BACKUP_EXT;
        static string CONNECTION_STRING_FORMAT = "Data Source={0};Version=3;";

        /// <summary>
        /// Migrates the configuration database, making a backup before it begins.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>A boolean, where <value>true</value> means the migration(s) were successful, and <value>false</value> means that an error occurred.</returns>
        public static void Migrate(Drey.Nut.INutConfiguration config)
        {
            string dbNameAndPath = Path.Combine(config.HordeBaseDirectory, CONFIG_FILE_NAME);

            var currentDb = new FileInfo(dbNameAndPath);
            var backupDb = Backup(currentDb);

            var ctx = new RunnerContext(new ConsoleAnnouncer())
            {
                ApplicationContext = string.Empty,
                Database = "sqlite",
                Connection = string.Format(CONNECTION_STRING_FORMAT, dbNameAndPath),
                Targets = new[] { "Drey.Configuration" }
            };

            try
            {
                var executor = new TaskExecutor(ctx);
                executor.Execute();
            }
            catch (Exception)
            {
                currentDb.Delete();
                if (!string.IsNullOrWhiteSpace(backupDb)) { File.Copy(backupDb, currentDb.FullName); }
                throw;
            }
        }

        /// <summary>
        /// Backups the current configuration database before a migration.
        /// </summary>
        /// <param name="currentDb">The current database.</param>
        /// <returns>a string containing the full path and filename to the backed up db3 file.</returns>
        private static string Backup(FileInfo currentDb)
        {
            if (!currentDb.Exists) { return string.Empty; }


            var dirPath = Path.GetDirectoryName(currentDb.FullName);
            var backupFile = Path.Combine(dirPath, string.Format(DB_BACKUP_FILENAME_FORMAT, DateTime.UtcNow));
            currentDb.CopyTo(backupFile);

            DirectoryInfo dbDir = new DirectoryInfo(dirPath);
            FileSystemInfo[] files = dbDir.GetFileSystemInfos();
            files.Where(f => f.Name.EndsWith(DB_BACKUP_EXT))
                .OrderByDescending(x => x.CreationTime)
                .Skip(MAX_BACKUP_FILES)
                .ToList()
                .ForEach(x => x.Delete());

            return backupFile;
        }
    }
}