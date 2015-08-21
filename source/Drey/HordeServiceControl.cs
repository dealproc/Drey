using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Drey
{
    public class HordeServiceControl : MarshalByRefObject
    {
        static readonly ILog _Log = LogProvider.For<HordeServiceControl>();

        ShellFactory _appFactory = new ShellFactory();
        INutConfiguration _nutConfiguration = new ApplicationHostNutConfiguration();
        List<Tuple<AppDomain, IShell>> _appInstances = new List<Tuple<AppDomain, IShell>>();

        public bool Start()
        {
            return StartupInstance(_nutConfiguration, DreyConstants.ConfigurationPackageName, string.Empty);
        }

        public bool Stop()
        {
            var packageIds = _appInstances.Select(x => x.Item2.Id).ToArray();
            packageIds.Apply(ShutdownInstance);
            _appInstances = null;

            return true;
        }

        void ShellRequestHandler(object sender, ShellRequestArgs e)
        {
            _Log.InfoFormat("'Shell Request' Event Received: {0}", e);

            switch (e.ActionToTake)
            {
                case ShellAction.Startup:
                    StartupInstance(e.ConfigurationManager, e.PackageId, e.Version);
                    break;
                case ShellAction.Shutdown:
                    ShutdownInstance(e.PackageId);
                    break;
                case ShellAction.Restart:
                    ShutdownInstance(e.PackageId);
                    StartupInstance(e.ConfigurationManager, e.PackageId, e.Version);
                    break;
                default:
                    _Log.ErrorFormat("Received an unknown action: {0}", e.ActionToTake);
                    break;
            }
        }

        bool StartupInstance(INutConfiguration configurationManager, string id, string version = "")
        {
            string packageDir = string.IsNullOrWhiteSpace(version)
                ?
                Utilities.PackageUtils.DiscoverPackage(id, _nutConfiguration.HordeBaseDirectory)
                :
                Utilities.PackageUtils.DiscoverPackage(id, _nutConfiguration.HordeBaseDirectory, version);

            var shell = _appFactory.Create(packageDir, configurationManager);
            if (shell == null)
            {
                _Log.Fatal("Did not create the configuration console.  app exiting.");
                return false;
            }

            _Log.Info("Configuration shell created.  Starting to listen for events.");
            shell.Item2.OnShellRequest += ShellRequestHandler;
            _appInstances.Add(shell);
            return true;
        }

        void ShutdownInstance(string id)
        {
            var instancesToShutdown = _appInstances.Where(i => i.Item2.Id == id).ToArray();
            instancesToShutdown.Apply(i =>
            {
                i.Item2.Shutdown();
                i.Item2.Dispose();
                AppDomain.Unload(i.Item1);
                _appInstances.Remove(i);
            });
        }
    }
}