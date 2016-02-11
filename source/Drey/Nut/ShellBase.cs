using Drey.Logging;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Nut
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.MarshalByRefObject" />
    /// <seealso cref="Drey.Nut.IShell" />
    /// <seealso cref="System.IDisposable" />
    public abstract class ShellBase : MarshalByRefObject, IShell, IDisposable
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();

        Task _keepAlive;

        /// <summary>
        /// Gets the log.
        /// </summary>
        protected static ILog Log { get { return _Log; } }

        /// <summary>
        /// The shell's Id.  This should be a 1-1 correlation with the nuget package id.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the package requires usage of the configuation storate utilities in Drey.
        /// </summary>
        public abstract bool RequiresConfigurationStorage { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ShellBase"/> is disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellBase"/> class.
        /// </summary>
        public ShellBase()
        {
            ConfigureLogging = (config) => { };
            Disposed = false;

            _keepAlive = Task.Factory.StartNew(() =>
            {
                while (!Disposed)
                {
                    Task.Delay(TimeSpan.FromSeconds(30)).Wait();
                    EmitShellRequest(new ShellRequestArgs()
                    {
                        ActionToTake = ShellAction.Heartbeat,
                        ConfigurationManager = null,
                        PackageId = Id,
                        Version = string.Empty,
                        RemoveOtherVersionsOnRestart = false
                    });
                }
            });
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="ShellBase"/> class.
        /// </summary>
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

        INutConfiguration _configurationManager;
        Sponsor<INutConfiguration> _configurationManagerSponsorship;
        /// <summary>
        /// Gets or sets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        /// <exception cref="System.Exception">Cannot manage multiple configuration managers in the shell.</exception>
        public INutConfiguration ConfigurationManager
        {
            get { return _configurationManager; }
            protected set
            {
                _configurationManager = value;
                if (_configurationManagerSponsorship != null && value.ShellControlsLifetimeSponsorship)
                {
                    throw new Exception("Cannot manage multiple configuration managers in the shell.");
                }
                if (_configurationManagerSponsorship == null && value.ShellControlsLifetimeSponsorship)
                {
                    _configurationManagerSponsorship = new Sponsor<INutConfiguration>(value);
                }
            }
        }

        /// <summary>
        /// An action, provided by the host, to allow the host to configure logging within the package's domain.
        /// </summary>
        public Action<INutConfiguration> ConfigureLogging { get; set; }

        /// <summary>
        /// Occurs when this shell needs the runtime to perform an operation on the shell's behalf.
        /// </summary>
        public event EventHandler<ShellRequestArgs> OnShellRequest;

        /// <summary>
        /// Gets or sets the shell request handler.
        /// </summary>
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

            _Log.Info("Setting up Server Certificate Validation.");
            ConfigurationManager.CertificateValidator.Initialize();

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) { return; }

            if (ConfigurationManager is IDisposable)
            {
                ((IDisposable)ConfigurationManager).Dispose();
            }

            if (_configurationManagerSponsorship != null)
            {
                _configurationManagerSponsorship.Dispose();
            }

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