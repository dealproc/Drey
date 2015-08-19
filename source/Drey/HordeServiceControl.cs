using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Drey
{
    public class HordeServiceControl
    {
        ShellFactory _appFactory = new ShellFactory();
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        IShell _configurationShell = null;

        public bool Start()
        {
            try
            {
                var packagePath = Utilities.PackageUtils.DiscoverPackage(DreyConstants.ConfigurationPackageName, _nutConfiguration.HordeBaseDirectory);
                _configurationShell = _appFactory.Create(packagePath, _nutConfiguration);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop()
        {
            _configurationShell.Shutdown().Wait();
            _configurationShell.Dispose();
            _configurationShell = null;

            return true;
        }
    }
}