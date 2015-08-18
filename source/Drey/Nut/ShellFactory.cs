using System;
using System.IO;

namespace Drey.Nut
{
    public class ShellFactory : MarshalByRefObject
    {
        public IShell Create(string assemblyPath, INutConfiguration config)
        {
            var startupOptions = DiscoverEntryDLL(assemblyPath);

            return startupOptions == null ? null : new Shell(startupOptions, config);
        }

        private ShellStartOptions DiscoverEntryDLL(string assemblyPath)
        {
            var path = Utilities.PathUtilities.ResolvePath(assemblyPath);

            var proxyType = typeof(DiscoverStartupDllProxy);
            var thisAssemblyPath = Utilities.PathUtilities.ResolvePath(proxyType.Assembly.GetName().CodeBase.Remove(0, 8), false);

            var domain = Utilities.AppDomainUtils.CreateDomain(Guid.NewGuid().ToString());

            try
            {
                foreach (var file in Directory.GetFiles(path, "*.dll"))
                {
                    var discoverProxy = (DiscoverStartupDllProxy)domain.CreateInstanceFromAndUnwrap(thisAssemblyPath, proxyType.FullName);

                    if (discoverProxy.IsStartupDll(file))
                    {
                        return discoverProxy.BuildOptions(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
            return null;
        }
    }
}