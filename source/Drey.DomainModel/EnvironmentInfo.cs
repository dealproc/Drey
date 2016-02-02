using System;
using System.IO;
using System.Reflection;
namespace Drey.DomainModel
{
    public class EnvironmentInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether [is64 bit operating system].
        /// </summary>
        public bool Is64BitOperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is64 bit process].
        /// </summary>
        public bool Is64BitProcess { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The human readable/understandable version (name) of the operating system.
        /// </summary>
        public string OSFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the os version.
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets the processor count.
        /// </summary>
        public int ProcessorCount { get; set; }

        /// <summary>
        /// Gets or sets the uptime.
        /// </summary>
        public long Uptime { get; set; }

        /// <summary>
        /// Gets or sets the IP v4 addresses.
        /// </summary>
        public string[] IPv4Addresses { get; set; }

        /// <summary>
        /// Gets or sets the physical memory usage of the process.
        /// </summary>
        public long WorkingSet64 { get; set; }

        /// <summary>
        /// Gets or sets the total memory (expressed as bytes).
        /// </summary>
        public ulong TotalMemoryBytes { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the percentage memory in use.
        /// </summary>
        public uint PercentageMemoryInUse { get; set; }

        /// <summary>
        /// Gets or sets the .NET Runtime version the process is executing on.
        /// </summary>
        public string EnvironmentVersion { get; set; }

        /// <summary>
        /// Gets or sets the registered database factories.
        /// <remarks>Useful information for troubleshooting client configuration errors.  we'll know if the driver is available.</remarks>
        /// </summary>
        public RegisteredDbProviderFactory[] RegisteredDbFactories { get; set; }

        public FrameworkInfo InstalledFrameworks { get; set; }
    }

    /// <summary>
    /// Describes a registered DbProviderFactory in the system.
    /// </summary>
    public class RegisteredDbProviderFactory
    {
        /// <summary>
        /// The name of the factory
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Human readable description of the provider factory.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The invariant name of the provider.
        /// <remarks>This is used when calling into the DbProviderFactories.GetFactory(string invariantName) method.</remarks>
        /// </summary>
        public string InvariantName { get; set; }
        /// <summary>
        /// The fully qualified assembly name.
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
    }

    public class NetworkInterfaceInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the network interface device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the network interface device.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is receive only.
        /// </summary>
        public bool IsReceiveOnly { get; set; }
    }

    public class FrameworkInfo
    {
        public FrameworkVersion[] NetFxVersions { get; set; }

        public bool HasNetFx45 { get; set; }
        public string NetFx45FriendlyVersion { get; set; }

        public FrameworkInfo()
        {
            HasNetFx45 = false;
            NetFx45FriendlyVersion = string.Empty;
            NetFxVersions = new FrameworkVersion[0];
        }
    }
    public class FrameworkVersion
    {
        public string CommonVersion { get; set; }
        public string BuildVersion { get; set; }
        public string ServicePack { get; set; }
    }
}
