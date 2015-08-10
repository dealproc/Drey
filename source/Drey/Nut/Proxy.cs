using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    class Proxy : MarshalByRefObject
    {
        public bool IsStartupDll(string path)
        {
            var asm = Assembly.LoadFrom(path);
            // there has to be a better way than this.
            var hasAttribute = Attribute.GetCustomAttributes(asm).Any(attr => attr.GetType().FullName == typeof(CrackingAttribute).FullName);
            return hasAttribute;
        }
        
        public ShellStartOptions BuildOptions(string path)
        {
            var asm = Assembly.LoadFrom(path);
            var attrib = Attribute.GetCustomAttributes(asm)
                .Where(attr => attr.GetType().FullName == typeof(CrackingAttribute).FullName)
                .First();

            var domainNameProperty = attrib.GetType().GetProperty("ApplicationDomainName");
            var requiresConfigStorageProperty = attrib.GetType().GetProperty("RequiresConfigurationStorage");
            var startupClassProperty = attrib.GetType().GetProperty("StartupClass");

            return new ShellStartOptions
            {
                DllPath = path,
                ApplicationDomainName = (string)domainNameProperty.GetValue(attrib),
                ProvideConfigurationOptions = (bool)requiresConfigStorageProperty.GetValue(attrib),
                AssemblyName = asm.FullName,
                StartupClass = ((Type)startupClassProperty.GetValue(attrib)).FullName
            };
        }

        public Assembly Load(string fileNameAndPath)
        {
            var asm = Assembly.LoadFile(fileNameAndPath);
            return asm;
        }
    }
}