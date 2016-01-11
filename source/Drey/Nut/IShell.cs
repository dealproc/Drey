using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Nut
{
    /// <summary>
    /// Shell interface.  This is the definition of a startup routine for a given package.
    /// </summary>
    public interface IShell : IDisposable
    {
        /// <summary>
        /// The shell's Id.  This should be a 1-1 correlation with the nuget package id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the package requires usage of the configuation storate utilities in Drey.
        /// </summary>
        bool RequiresConfigurationStorage { get; }

        /// <summary>
        /// Provides an ability for package authors to set default application settings for their package.
        /// </summary>
        IEnumerable<DefaultAppSetting> AppSettingDefaults { get; }

        /// <summary>
        /// Provides an ability for package authors to set default connection strings for their package, if necessary.
        /// </summary>
        IEnumerable<DefaultConnectionString> ConnectionStringDefaults { get; }

        /// <summary>
        /// Occurs when [on shell request].
        /// </summary>
        event EventHandler<ShellRequestArgs> OnShellRequest;

        /// <summary>
        /// Gets or sets the shell request handler.
        /// </summary>
        EventHandler<ShellRequestArgs> ShellRequestHandler { get; set; }

        /// <summary>
        /// Startup routine for your package.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <returns></returns>
        bool Startup(INutConfiguration configurationManager);

        /// <summary>
        /// Shutdown routine for your package.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// An action, provided by the host, to allow the host to configure logging within the package's domain.
        /// </summary>
        Action<INutConfiguration> ConfigureLogging { get; set; }
    }
}