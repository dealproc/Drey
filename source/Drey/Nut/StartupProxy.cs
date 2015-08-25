using System;
using System.Reflection;

namespace Drey.Nut
{
    class StartupProxy : ProxyBase
    {
        public StartupProxy(string pathToAppPackage) : base(pathToAppPackage) { }

        /// <summary>
        /// Creates an IShell instance within a child app domain, given the library's full path and class name.
        /// </summary>
        /// <param name="pathAndFile">The path and file.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IShell Build(string pathAndFile, string typeName, params object[] args)
        {
            var assembly = Assembly.LoadFrom(pathAndFile);
            var type = assembly.GetType(typeName);
            return (IShell)Activator.CreateInstance(type, args);
        }
    }
}