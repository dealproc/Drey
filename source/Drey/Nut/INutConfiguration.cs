using Drey.CertificateValidation;
namespace Drey.Nut
{
    public interface INutConfiguration
    {
        /// <summary>
        /// Provides access to the runtime's global settings store.
        /// </summary>
        IGlobalSettings GlobalSettings { get; }

        /// <summary>
        /// Provides access to the package's application settings store.
        /// </summary>
        IApplicationSettings ApplicationSettings { get; }

        /// <summary>
        /// Provides access to the package's ConnectionStrings store.
        /// </summary>
        IConnectionStrings ConnectionStrings { get; }

        /// <summary>
        /// Gets the Runtime's Working Directory.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Gets the base directory for the location where all unpacked packages are located.  This is a subfolder of the WorkingDirectory folder.
        /// </summary>
        string HoardeBaseDirectory { get; }

        /// <summary>
        /// Gets the base directory for the location that custom dlls/plugins are located.  This is a subfolder of the WorkingDirectory folder.
        /// </summary>
        string PluginsBaseDirectory { get; }

        /// <summary>
        /// Gets the logs directory.
        /// </summary>
        string LogsDirectory { get; }

        /// <summary>
        /// Gets the mode that the runtime is executing in.  Possible values are Development and Production.
        /// </summary>
        ExecutionMode Mode { get; }

        /// <summary>
        /// Gets the log verbosity.
        /// </summary>
        string LogVerbosity { get; }

        /// <summary>
        /// Gets the certificate validator.
        /// </summary>
        ICertificateValidation CertificateValidator { get; }
    }
}