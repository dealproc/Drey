using Drey.Nut;
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

        protected void Execute(Action<IDbConnection> work)
        {
            using (var cn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = string.Format(CONNECTION_STRING_FORMAT, Path.Combine(_configurationManager.HordeBaseDirectory, CONFIG_FILE_NAME));
                cn.Open();

                work(cn);
            }
        }
        protected T Execute<T>(Func<IDbConnection, T> work)
        {
            using (var cn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = string.Format(CONNECTION_STRING_FORMAT, Path.Combine(_configurationManager.HordeBaseDirectory, CONFIG_FILE_NAME));
                cn.Open();

                return work(cn);
            }
        }
     
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