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
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<IShell> _shells = new List<IShell>();

        public bool Start()
        {
            _nutConfiguration.EventBus.Subscribe(this);

            try
            {
                var packagePath = DiscoverPackage(DreyConstants.ConfigurationPackageName);

                var shell = ShellFactory.Create(packagePath, _nutConfiguration);
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
            var package = DiscoverPackage(message.PackageId);
            if (!string.IsNullOrEmpty(package))
            {
                var shell = ShellFactory.Create(package, _nutConfiguration);
                _shells.Add(shell);
                _nutConfiguration.EventBus.Publish(new PackageEvents.Loaded { PackageId = message.PackageId, InstanceId = shell.InstanceId });
            }
        }

        public void Handle(PackageEvents.Shutdown message)
        {
            try
            {
                var shell = _shells.Where(s => s.InstanceId == message.InstanceId).Single();
                _shells.Remove(shell);
                shell.Dispose();
                _nutConfiguration.EventBus.Publish(new PackageEvents.Disposed { InstanceId = shell.InstanceId });
            }
            catch
            {
                // what to do?
            }
        }

        private string DiscoverPackage(string packageId)
        {
            // discover the main horde folder
            var configurationPath = Utilities.PathUtilities.ResolvePath(Path.Combine(_nutConfiguration.HordeBaseDirectory, packageId));

            // discover the latest version
            var versionFolders = Directory.GetDirectories(configurationPath).Select(dir => (new DirectoryInfo(dir)).Name);
            var versions = versionFolders.Select(ver => new Version(ver));
            var latestVersion = versionFolders.OrderByDescending(x => x).First();

            return Path.Combine(configurationPath, latestVersion.ToString());
        }
    }
}