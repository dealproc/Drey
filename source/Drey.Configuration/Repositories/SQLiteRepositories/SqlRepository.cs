using Drey.Nut;
using Drey.Utilities;

using System;
using System.Data;
using System.IO;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public class SqlRepository
    {
        const string CONNECTION_STRING_FORMAT = "Data Source={0};Version=3;";
        const string CONFIG_FILE_NAME = "config.db3";

        INutConfiguration _configurationManager;

        public SqlRepository(INutConfiguration configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Executes the specified work.
        /// </summary>
        /// <param name="work">The work.</param>
        protected void Execute(Action<IDbConnection> work)
        {
            using (var cn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = string.Format(CONNECTION_STRING_FORMAT, Path.Combine(_configurationManager.WorkingDirectory, CONFIG_FILE_NAME).NormalizePathSeparator());
                cn.Open();

                work(cn);
            }
        }
        
        /// <summary>
        /// Executes the specified work.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work">The work.</param>
        /// <returns></returns>
        protected T Execute<T>(Func<IDbConnection, T> work)
        {
            using (var cn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = string.Format(CONNECTION_STRING_FORMAT, Path.Combine(_configurationManager.WorkingDirectory, CONFIG_FILE_NAME).NormalizePathSeparator());
                cn.Open();

                return work(cn);
            }
        }

        /// <summary>
        /// Executes the with transaction.
        /// </summary>
        /// <param name="work">The work.</param>
        protected void ExecuteWithTransaction(Action<IDbConnection> work)
        {
            Execute((cn) =>
            {
                using (var tx = cn.BeginTransaction())
                {
                    work(cn);
                    tx.Commit();
                }
            });
        }
    }
}