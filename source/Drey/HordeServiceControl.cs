using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey
{
    public class HordeServiceControl
    {
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<IShell> _shells = new List<IShell>();
        string _dreyConfigurationPackagePath;

        public bool Start()
        {
            DiscoverConfigurationPackage();

            var shell = ShellFactory.Create(_dreyConfigurationPackagePath, _nutConfiguration);
            _shells.Add(shell);

            foreach (var package in DiscoverPackages())
            {
                _shells.Add(ShellFactory.Create(package, _nutConfiguration));
            }

            return true;
        }

        public bool Stop()
        {
            foreach (var shell in _shells)
            {
                shell.Dispose();
            }

            return true;
        }

        private IEnumerable<string> DiscoverPackages()
        {
            var hoarde = Utilities.PathUtilities.ResolvePath(_nutConfiguration.HordeBaseDirectory);
            return Directory.GetDirectories(hoarde)
                .Where(dir => !dir.EndsWith(DreyConstants.ConfigurationPackageName))
                .Select(pkgdir =>
                {
                    var versionFolders = Directory.GetDirectories(pkgdir).Select(dir => (new DirectoryInfo(dir)).Name);
                    var versions = versionFolders.Select(ver => new Version(ver));
                    var latestVersion = versionFolders.OrderByDescending(x => x).First();

                    return Path.Combine(pkgdir, latestVersion.ToString());
                });
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
    }
}