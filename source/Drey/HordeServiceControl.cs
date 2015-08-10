using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Topshelf;

namespace Drey
{
    public class HordeServiceControl : ServiceControl
    {
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<IShell> _shells = new List<IShell>();
        string _dreyConfigurationPackagePath;

        public bool Start(HostControl hostControl)
        {
            DiscoverConfigurationPackage();

            var shell = ShellFactory.Create(_dreyConfigurationPackagePath, _nutConfiguration);
            shell.ShellCallback += shell_ShellCallback;
            _shells.Add(shell);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            foreach (var shell in _shells)
            {
                shell.ShellCallback -= shell_ShellCallback;
                shell.Dispose();
            }
            
            return true;
        }

        void shell_ShellCallback(object sender, ShellEventArgs e)
        {

        }

        private void DiscoverConfigurationPackage()
        {
            // discover the main horde folder
            var configurationPath = Path.Combine(_nutConfiguration.HordeBaseDirectory, "drey.configuration");

            if (configurationPath.StartsWith("~/"))
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Remove(0, 8)) + "\\";
                configurationPath = configurationPath.Replace("~/", dir);
            }

            if (!configurationPath.EndsWith("\\"))
            {
                configurationPath += "\\";
            }

            configurationPath = configurationPath.Replace("/", "\\");

            // discover the latest version
            var versionFolders = Directory.GetDirectories(configurationPath).Select(dir => (new DirectoryInfo(dir)).Name);
            var versions = versionFolders.Select(ver => new Version(ver));
            var latestVersion = versionFolders.OrderByDescending(x => x).First();

            _dreyConfigurationPackagePath = Path.Combine(configurationPath, latestVersion.ToString());
        }
    }
}