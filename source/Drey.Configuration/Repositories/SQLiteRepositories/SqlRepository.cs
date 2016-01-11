using Drey.Nut;
using Drey.Utilities;

using Mono.Data.Sqlite;

using System;
using System.Data;
using System.IO;
using System.Security.Permissions;

namespace Drey.Configuration.Repositories.SQLiteRepositories
{
    public abstract class SqlRepository : MarshalByRefObject
    {
        const string CONNECTION_STRING_FORMAT = "Data Source=\"{0}\";Version=3;";
        const string CONFIG_FILE_NAME = "config.db3";

        INutConfiguration _configurationManager;

        Func<INutConfiguration, string> ConnectionStringBuilder = (config) => string.Format(CONNECTION_STRING_FORMAT, PathUtilities.MapPath(Path.Combine(config.WorkingDirectory, CONFIG_FILE_NAME), false));

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepository"/> class.
        /// <remarks>Used by IoC container.</remarks>
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public SqlRepository(INutConfiguration configurationManager)
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepository"/> class.
        /// <remarks>Used for integration testing.</remarks>
        /// </summary>
        /// <param name="databaseNameAndPath">The database name and path.</param>
        public SqlRepository(string databaseNameAndPath)
        {
            ConnectionStringBuilder = (config) => string.Format(CONNECTION_STRING_FORMAT, databaseNameAndPath);
        }

        /// <summary>
        /// Executes the specified work.
        /// </summary>
        /// <param name="work">The work.</param>
        protected void Execute(Action<IDbConnection> work)
        {
            using (var cn = SqliteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = ConnectionStringBuilder.Invoke(_configurationManager);
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
            using (var cn = SqliteFactory.Instance.CreateConnection())
            {
                cn.ConnectionString = ConnectionStringBuilder.Invoke(_configurationManager);
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

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}