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
            foreach (var file in Directory.GetFiles(assemblyPath, "*.dll"))
            {
                var asmToReflect = Assembly.LoadFrom(file);
                Type entryType = asmToReflect.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IShell)));
                if (entryType != null)
                {
                    return new Tuple<string, string>(file, entryType.FullName);
                }
            }
            return null;
        }
    }
}