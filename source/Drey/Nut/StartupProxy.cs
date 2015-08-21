using System;
using System.Reflection;

namespace Drey.Nut
{
    class StartupProxy : ProxyBase
    {
        public StartupProxy(string pathToAppPackage) : base(pathToAppPackage) { }

        public IShell Build(string pathAndFile, string typeName, params object[] args)
        {
            var assembly = Assembly.LoadFrom(pathAndFile);
            var type = assembly.GetType(typeName);
            return (IShell)Activator.CreateInstance(type, args);
        }
    }
}