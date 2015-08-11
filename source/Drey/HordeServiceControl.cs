using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drey
{
    public class HordeServiceControl : IHandle<PackageEvents.Load>, IHandle<PackageEvents.Shutdown>
    {
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<IShell> _shells = new List<IShell>();
        string _dreyConfigurationPackagePath;

        public bool Start()
        {
            _nutConfiguration.EventBus.Subscribe(this);

            DiscoverConfigurationPackage();

            var shell = ShellFactory.Create(_dreyConfigurationPackagePath, _nutConfiguration);
            _shells.Add(shell);

            return true;
        }

        public bool Stop()
        {
            foreach (var shell in _shells)
            {
                shell.Dispose();
            }

            _nutConfiguration.EventBus.Unsubscribe(this);

            return true;
        }

        private void DiscoverConfigurationPackage()
        {
            // discover the main horde folder
            var configurationPath = Utilities.PathUtilities.ResolvePath(Path.Combine(_nutConfiguration.HordeBaseDirectory, DreyConstants.ConfigurationPackageName));

            // discover the latest version
            var versionFolders = Directory.GetDirectories(configurationPath).Select(dir => (new DirectoryInfo(dir)).Name);
            var versions = versionFolders.Select(ver => new Version(ver));
            var latestVersion = versionFolders.OrderByDescending(x => x).First();

            _dreyConfigurationPackagePath = Path.Combine(configurationPath, latestVersion.ToString());
        }

        public void Handle(PackageEvents.Load message)
        {
            Console.WriteLine("Load called.");
        }

        public void Handle(PackageEvents.Shutdown message)
        {
            Console.WriteLine("Shutdown called.");
        }
    }
}