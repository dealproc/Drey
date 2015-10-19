using Drey.Configuration.Infrastructure;
using Drey.Logging;

using Microsoft.AspNet.SignalR.Client;
using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Timers;

namespace Drey.Configuration.ServiceModel
{
    class ReportHealthService : IReportPeriodically, IDisposable
    {
        static readonly ILog _log = LogProvider.For<ReportHealthService>();

        bool _disposed = false;

        IHubConnectionManager _hubConnectionManager;
        IHubProxy _runtimeHubProxy;
        Timer _reportHealthTrigger;
        DomainModel.RegisteredDbProviderFactory[] _registeredDbFactories;
        DomainModel.FrameworkInfo _frameworkInfo;

        public ReportHealthService(IHubConnectionManager hubConnectionManager)
        {
            _reportHealthTrigger = new Timer();
            _reportHealthTrigger.Elapsed += reportHealthTrigger_Elapsed;
            _reportHealthTrigger.Interval = 15000;
        }

        public void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy)
        {
            _hubConnectionManager = hubConnectionManager;
            _runtimeHubProxy = runtimeHubProxy;

            DiscoverDotNetFrameworks();
            _registeredDbFactories = DbProviderFactories.GetFactoryClasses().Select().Select(x => new DomainModel.RegisteredDbProviderFactory
            {
                Name = x["Name"].ToString(),
                Description = x["Description"].ToString(),
                InvariantName = x["InvariantName"].ToString(),
                AssemblyQualifiedName = x["AssemblyQualifiedName"].ToString()
            }).ToArray();

            _reportHealthTrigger.Start();
        }

        public void Stop()
        {
            _reportHealthTrigger.Stop();
        }

        /// <summary>
        /// Reports to the broker the current health information of this client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void reportHealthTrigger_Elapsed(object sender, ElapsedEventArgs e)
        {
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
                            // should gather the main netfx info.
                            frameworkVersions.Add(new DomainModel.FrameworkVersion
                            {
                                CommonVersion = versionKeyName,
                                BuildVersion = name,
                                ServicePack = (sp != "" && install == "1") ? "SP" + sp : string.Empty
                            });
                        }
                        else
                        {
                            // must be the netfx 4.0 node
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

        public void Dispose()
        {
            Dispose(true);
            _disposed = true;

            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_reportHealthTrigger != null)
            {
                _reportHealthTrigger.Dispose();
                _reportHealthTrigger = null;
            }

            if (!disposing || _disposed) { return; }
        }
    }
}
