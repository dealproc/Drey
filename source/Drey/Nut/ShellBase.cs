using Drey.Logging;

using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Drey.Nut
{
    public abstract class ShellBase : MarshalByRefObject, IShell
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();
        protected static ILog Log { get { return _Log; } }

        public abstract string Id { get; }
        public abstract bool RequiresConfigurationStorage { get; }
        protected bool Disposed { get; private set; }

        public ShellBase()
        {
            ConfigureLogging = (config) => { };
            Disposed = false;
        }
        ~ShellBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the application setting defaults.
        /// </summary>
        public abstract IEnumerable<DefaultAppSetting> AppSettingDefaults { get; }

        /// <summary>
        /// Gets the connection string defaults.
        /// </summary>
        public abstract IEnumerable<DefaultConnectionString> ConnectionStringDefaults { get; }

        public INutConfiguration ConfigurationManager { get; protected set; }

        public Action<INutConfiguration> ConfigureLogging { get; set; }

        /// <summary>
        /// Occurs when this shell needs the runtime to perform an operation on the shell's behalf.
        /// </summary>
        public event EventHandler<ShellRequestArgs> OnShellRequest;

        /// <summary>
        /// The startup routine for the applet.  Think of this like `static main(string args[]) { ... }`.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public virtual void Startup(INutConfiguration configurationManager)
        {
            Log.InfoFormat("{packageName} is starting in {mode}.", this.Id, configurationManager.Mode);
            ConfigurationManager = configurationManager;
            ConfigureLogging(configurationManager);

            _Log.Info("Registering default app settings.");
            AppSettingDefaults.Apply((DefaultAppSetting setting) =>
            {
                if (!configurationManager.ApplicationSettings.Exists(setting.Key)) { configurationManager.ApplicationSettings.Register(setting.Key, setting.Value); }
            });

            _Log.Info("Registering default connection string(s).");
            ConnectionStringDefaults.Apply((DefaultConnectionString connStr) =>
            {
                if (!configurationManager.ConnectionStrings.Exists(connStr.Name)) { configurationManager.ConnectionStrings.Register(connStr.Name, connStr.ConnectionString, connStr.ProviderName); }
            });
        }

        /// <summary>
        /// Should your app need a specific shutdown routine, you will override this method and impement it.
        /// </summary>
        public virtual void Shutdown() { }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) { return; }

            Disposed = true;
        }

        /// <summary>
        /// Emits the shell request.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void EmitShellRequest(ShellRequestArgs args)
        {
            var handler = OnShellRequest;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}