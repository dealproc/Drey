using System;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    class DiscoverStartupDllProxy : MarshalByRefObject
    {
        public bool IsStartupDll(string path)
        {
            var asm = Assembly.LoadFrom(path);
            var attrib = asm.GetCustomAttribute<CrackingAttribute>();
            return attrib != null;
        }

        public ShellStartOptions BuildOptions(string path)
        {
            var asm = Assembly.LoadFrom(path);
            var attrib = Attribute.GetCustomAttribute(asm, typeof(CrackingAttribute)) as CrackingAttribute;

            return new ShellStartOptions
            {
                DllPath = path,
                DisplayAs = attrib.DisplayAs,
                ApplicationDomainName = attrib.ApplicationDomainName,
                AssemblyName = asm.FullName,
                PackageId = attrib.PackageId,
                StartupClass = attrib.StartupClass.FullName,
                ProvideConfigurationOptions = attrib.RequiresConfigurationStorage
            };
        }

        public Assembly Load(string fileNameAndPath)
        {
            var asm = Assembly.LoadFile(fileNameAndPath);
            return asm;
        }
    }
}