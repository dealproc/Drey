using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace Drey.Nut
{
    public static class ShellFactory
    {
        public static IShell Create(string assemblyPath, INutConfiguration config)
        {
            var startupOptions = DiscoverEntryDLL(assemblyPath);

            return startupOptions == null ? null : new Shell(startupOptions, config);
        }

        private static ShellStartOptions DiscoverEntryDLL(string assemblyPath)
        {
            var path = assemblyPath;
            if (path.StartsWith("~/"))
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Remove(0, 8)) + "\\";
                path = path.Replace("~/", dir);
            }

            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }

            path = path.Replace("/", "\\");

            var domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = path;
            domainSetup.PrivateBinPath = path;
            Evidence adEvidence = AppDomain.CurrentDomain.Evidence;

            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), adEvidence, domainSetup);

            try
            {
                foreach (var file in Directory.GetFiles(path, "*.dll"))
                {
                    var type = typeof(Proxy);
                    var thisAssemblyPath = Path.Combine(path, type.Assembly.GetName().CodeBase.Remove(0, 8));
                    var loader = (Proxy)domain.CreateInstanceFromAndUnwrap(thisAssemblyPath, type.FullName);

                    if (loader.IsStartupDll(file))
                    {
                        return loader.BuildOptions(file);
                    }
                }
            }
            finally
            {
                AppDomain.Unload(domain);
            }
            return null;
        }
    }
}