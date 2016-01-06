using Drey.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace Drey.Nut
{
    public abstract class ShellBase : MarshalByRefObject, IShell, IDisposable
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();
        protected static ILog Log { get { return _Log; } }

        public abstract string Id { get; }
        public abstract bool RequiresConfigurationStorage { get; }
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Gets an enumeration of nested <see cref="MarshalByRefObject"/> objects.
        /// </summary>
        protected virtual IEnumerable<MarshalByRefObject> NestedMarshalByRefObjects
        {
            get { return Enumerable.Empty<MarshalByRefObject>(); }
        }

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
        public EventHandler<ShellRequestArgs> ShellRequestHandler { get; set; }

        /// <summary>
        /// The startup routine for the applet.  Think of this like `static main(string args[]) { ... }`.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public virtual bool Startup(INutConfiguration configurationManager)
        {
            Log.InfoFormat("{packageName} is starting in {mode}.", this.Id, configurationManager.Mode);
            ConfigurationManager = configurationManager;
            if (ConfigureLogging != null)
            {
                ConfigureLogging(configurationManager);
            }

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

            return true;
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
            GC.SuppressFinalize(this);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            //
            // Returning null designates an infinite non-expiring lease.
            // We must therefore ensure that RemotingServices.Disconnect() is called when
            // it's no longer needed otherwise there will be a memory leak.
            //
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) { return; }

            if (ConfigurationManager is IDisposable)
            {
                ((IDisposable)ConfigurationManager).Dispose();
            }

            Disconnect();

            Disposed = true;
        }

        private void Disconnect()
        {
            RemotingServices.Disconnect(this);

            foreach (var tmp in NestedMarshalByRefObjects)
            {
                RemotingServices.Disconnect(tmp);
            }
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