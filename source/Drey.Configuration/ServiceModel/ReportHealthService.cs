using Drey.Configuration.Infrastructure;
using Drey.Logging;
using Drey.Nut;

using Microsoft.AspNet.SignalR.Client;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Timers;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Reports health statistics to the server, for display and diagnostic usage.
    /// </summary>
    class ReportHealthService : IReportPeriodically, IDisposable
    {
        static readonly ILog _log = LogProvider.For<ReportHealthService>();

        static INutConfiguration _configurationManager;

        bool _disposed = false;

        IHubConnectionManager _hubConnectionManager;
        IHubProxy _runtimeHubProxy;
        Timer _reportHealthTrigger;
        DomainModel.RegisteredDbProviderFactory[] _registeredDbFactories;
        DomainModel.FrameworkInfo _frameworkInfo;

        public ReportHealthService(INutConfiguration configurationManager)
        {
            _configurationManager = configurationManager;

            _reportHealthTrigger = new Timer();
            _reportHealthTrigger.Elapsed += reportHealthTrigger_Elapsed;
            _reportHealthTrigger.Interval = 15000;
        }

        /// <summary>
        /// Starts the Health Service.
        /// </summary>
        /// <param name="hubConnectionManager">The hub connection manager.</param>
        /// <param name="runtimeHubProxy">The runtime hub proxy.</param>
        public void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy)
        {
            _log.Info("Starting 'Report Health Service'.");
            _hubConnectionManager = hubConnectionManager;
            _runtimeHubProxy = runtimeHubProxy;

            try
            {
                DiscoverDotNetFrameworks();
            }
            catch (SecurityException secEx)
            {
                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _frameworkInfo.NetFxVersions = new[] {
                        new DomainModel.FrameworkVersion
                        {
                            BuildVersion = Environment.Version.ToString(),
                            CommonVersion = string.Format("Mono on {0}", Environment.OSVersion.Platform),
                            ServicePack = string.Empty
                        }
                    };

                }
                else
                {
                    _log.WarnException("Security exception while reading registry", secEx);
                }
            }
            catch (NullReferenceException nrEx)
            {
                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _frameworkInfo.NetFxVersions = new[] {
                        new DomainModel.FrameworkVersion
                        {
                            BuildVersion = Environment.Version.ToString(),
                            CommonVersion = string.Format("Mono on {0}", Environment.OSVersion.Platform),
                            ServicePack = string.Empty
                        }
                    };

                }
                else {
                    _log.WarnException("Null Reference exception while reading from registry", nrEx);
                }
            }

            try
            {
                _registeredDbFactories = DbProviderFactories.GetFactoryClasses().Select().Select(x => new DomainModel.RegisteredDbProviderFactory
                {
                    Name = x["Name"].ToString(),
                    Description = x["Description"].ToString(),
                    InvariantName = x["InvariantName"].ToString(),
                    AssemblyQualifiedName = x["AssemblyQualifiedName"].ToString()
                }).ToArray();
            }
            catch (NullReferenceException)
            {
                _registeredDbFactories = new DomainModel.RegisteredDbProviderFactory[0];
            }

            _log.Info("Starting 'Report Health Info' trigger.");
            _reportHealthTrigger.Start();
        }

        /// <summary>
        /// Stops the Health Service.
        /// </summary>
        public void Stop()
        {
            _log.Info("'Report Health Info' trigger stopping.");
            if (_reportHealthTrigger != null)
            {
                _reportHealthTrigger.Stop();
            }
        }

        /// <summary>
        /// Reports to the broker the current health information of this client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void reportHealthTrigger_Elapsed(object sender, ElapsedEventArgs e)
        {
            MemoryStatusEx memStatus = null;

            try
            {
                _log.Debug("Reading extended memory status from kernel32.dll");
                memStatus = MemoryStatusEx.MemoryInfo;
            }
            catch (Exception ex)
            {
                _log.ErrorException("Error retrieving extended memory information.", ex);
            }

            if (_hubConnectionManager.State != ConnectionState.Connected)
            {
                _log.Info("Connection has not been established with broker.  Could not report health statistics.");
                return;
            }

            _log.Debug("Resolving local IP addresses on all networks.");
            var na = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                .Where(ha => ha.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(x => x.ToString())
                .ToArray();

            _log.Debug("Resolving human readable OS name.");
            var osFriendlyName = (from x in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
                                  .Get()
                                  .OfType<ManagementObject>()
                                  select x.GetPropertyValue("Caption")).First();

            _log.Debug("Building environment information to report to broker.");
            var ei = new DomainModel.EnvironmentInfo
            {
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                MachineName = Environment.MachineName,
                OSFriendlyName = (osFriendlyName ?? "Unknown").ToString(),
                OSVersion = Environment.OSVersion.VersionString,
                ProcessorCount = Environment.ProcessorCount,
                Uptime = Environment.TickCount,
                IPv4Addresses = na,
                WorkingSet64 = Process.GetCurrentProcess().WorkingSet64,

                ExecutablePath = Utilities.PathUtilities.MapPath(_configurationManager.WorkingDirectory),
                WorkingPath = Utilities.PathUtilities.MapPath(_configurationManager.WorkingDirectory),
                LogsPath = Utilities.PathUtilities.MapPath(_configurationManager.LogsDirectory),
                PluginsPath = Utilities.PathUtilities.MapPath(_configurationManager.PluginsBaseDirectory),

                // Note: These may fail on Linux boxes.  May need to build a "provider" model for each platform.
                PercentageMemoryInUse = memStatus == null ? 0 : memStatus.MemoryLoad,
                TotalMemoryBytes = memStatus == null ? 0 : memStatus.TotalPhysical,

                EnvironmentVersion = Environment.Version.ToString(),
                RegisteredDbFactories = _registeredDbFactories,
                InstalledFrameworks = _frameworkInfo
            };

            _log.Debug("Returning environment information to broker.");
            _runtimeHubProxy.Invoke("ReportHealth", ei);
        }

        private void DiscoverDotNetFrameworks()
        {
            _frameworkInfo = new DomainModel.FrameworkInfo();

            _log.Info("Discovering installed .net frameworks.");

            List<DomainModel.FrameworkVersion> frameworkVersions = new List<DomainModel.FrameworkVersion>();

            // NetFx 1.0 - 3.5
            using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\", false))
            {
                foreach (var versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);

                        string name = versionKey.GetValue("Version", "").ToString();
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();

                        if (name != string.Empty)
                        {
                            // should gather the main .NET Framework info.
                            frameworkVersions.Add(new DomainModel.FrameworkVersion
                            {
                                CommonVersion = versionKeyName,
                                BuildVersion = name,
                                ServicePack = (sp != "" && install == "1") ? "SP" + sp : string.Empty
                            });
                        }
                        else
                        {
                            // must be the .NET Framework 4.0 node
                            foreach (string subKeyName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                                name = subKey.GetValue("Version", string.Empty).ToString();

                                int releaseKey = (int)subKey.GetValue("Release", 0);

                                if (string.IsNullOrWhiteSpace(name) || releaseKey == 0) { continue; }

                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    sp = subKey.GetValue("SP", string.Empty).ToString();
                                }
                                else
                                {
                                    sp = string.Empty;
                                }

                                frameworkVersions.Add(new DomainModel.FrameworkVersion
                                {
                                    CommonVersion = versionKeyName + "-" + subKeyName,
                                    BuildVersion = name,
                                    ServicePack = string.IsNullOrWhiteSpace(sp) ? "" : "SP" + sp
                                });


                                if (releaseKey >= 393273)
                                {
                                    _frameworkInfo.HasNetFx45 = true;
                                    _frameworkInfo.NetFx45FriendlyVersion = "4.6 RC or later";
                                }
                                else if ((releaseKey >= 379893))
                                {
                                    _frameworkInfo.HasNetFx45 = true;
                                    _frameworkInfo.NetFx45FriendlyVersion = "4.5.2 or later";
                                }
                                else if ((releaseKey >= 378675))
                                {
                                    _frameworkInfo.HasNetFx45 = true;
                                    _frameworkInfo.NetFx45FriendlyVersion = "4.5.1 or later";
                                }
                                else if ((releaseKey >= 378389))
                                {
                                    _frameworkInfo.HasNetFx45 = true;
                                    _frameworkInfo.NetFx45FriendlyVersion = "4.5 or later";
                                }
                            }
                        }
                    }
                }
            }

            _frameworkInfo.NetFxVersions = frameworkVersions.ToArray();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            _log.Debug("Disposing 'Report Health Trigger'.");

            if (!disposing || _disposed) { return; }

            if (_reportHealthTrigger != null)
            {
                _reportHealthTrigger.Dispose();
                _reportHealthTrigger = null;
            }
            _disposed = true;
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class MemoryStatusEx
    {
        public uint Length;

        public uint MemoryLoad;

        public ulong TotalPhysical;
        public ulong AvailablePhysical;

        public ulong TotalPageFile;
        public ulong AvailablePageFile;

        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;

        public MemoryStatusEx()
        {
            Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
        }

        public static MemoryStatusEx MemoryInfo
        {
            get
            {
                MemoryStatusEx status = new MemoryStatusEx();
                GlobalMemoryStatusEx(status);
                return status;
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);
    }
}
