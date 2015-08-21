using Drey.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    class DiscoverStartupDllProxy : ProxyBase
    {
        public DiscoverStartupDllProxy(string pathToAppPackage) : base(pathToAppPackage) { }

        public Tuple<string, string> DiscoverEntryDll(string assemblyPath)
        {
            Log.Info("Attempting to discover entry dll.");
            foreach (var file in Directory.GetFiles(assemblyPath, "*.dll"))
            {
                Log.DebugFormat("--Inspecting: {0}", file);
                var asmToReflect = Assembly.LoadFrom(file);
                Type entryType = asmToReflect.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IShell)));
                if (entryType != null)
                {
                    Log.DebugFormat("--Found entry dll: {0}|{1}", file, entryType.FullName);
                    Log.InfoFormat("Entry Dll discovered: {0}", file);
                    return new Tuple<string, string>(file, entryType.FullName);
                }
                else
                {
                    Log.Debug("--Dll did not contain a suitable class for startup.");
                }
            }

            Log.Info("No dll was found that contained a suitable entry point for instantiation.");
            return null;
        }
    }
}