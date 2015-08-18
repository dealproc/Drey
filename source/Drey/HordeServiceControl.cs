using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Drey
{
    public class HordeServiceControl : IHandle<PackageEvents.Load>, IHandle<PackageEvents.Shutdown>
    {
        ShellFactory _appFactory = new ShellFactory();
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<IShell> _shells = new List<IShell>();

        public bool Start()
        {
            _nutConfiguration.EventBus.Subscribe(this);

            try
            {
                var packagePath = DiscoverPackage(DreyConstants.ConfigurationPackageName);

                var shell = _appFactory.Create(packagePath, _nutConfiguration);
                _shells.Add(shell);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop()
        {
            Task.WhenAll(_shells.Select(s => s.Shutdown()).ToArray()).Wait();
            foreach (var shell in _shells)
            {
                shell.Dispose();
            }

            _nutConfiguration.EventBus.Unsubscribe(this);

            return true;
        }

        public void Handle(PackageEvents.Load message)
        {
            Console.WriteLine("Loading package: {0}", message.PackageId);

            var package = DiscoverPackage(message.PackageId);
            if (!string.IsNullOrEmpty(package))
            {
                Console.WriteLine("Package found... loading");
                var shell = _appFactory.Create(package, message.ConfigurationManager);
                _shells.Add(shell);
                _nutConfiguration.EventBus.Publish(new PackageEvents.Loaded { PackageId = message.PackageId });
            }
            else
            {
                Console.WriteLine("Package was not found.");
            }
        }

        public void Handle(PackageEvents.Shutdown message)
        {
            try
            {
                var shell = _shells.Where(s => s.PackageId == message.PackageId).Single();
                _shells.Remove(shell);
                shell.Dispose();
                _nutConfiguration.EventBus.Publish(new PackageEvents.Disposed { PackageId = shell.PackageId });
            }
            catch
            {
                // what to do?
            }
        }

        private string DiscoverPackage(string packageId)
        {
            // discover the main horde folder
            var configurationPath = Utilities.PathUtilities.ResolvePath(_nutConfiguration.HordeBaseDirectory);

            // discover the latest version
            var versionFolders = Directory.GetDirectories(configurationPath, packageId + "*").Select(dir => (new DirectoryInfo(dir)).Name);
            var versions = versionFolders.Select(ver => new Version(ver.Replace(packageId + "-", string.Empty)));
            var latestVersion = versions.OrderByDescending(x => x).First();

            return Path.Combine(configurationPath, string.Format("{0}-{1}", packageId, latestVersion.ToString()));
        }
    }
}