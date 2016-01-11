using System;
using System.IO;
using System.Reflection;

namespace Drey.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets an assembly's location on disc.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static string GetDirectoryLocation(this Assembly assembly)
        {
            return Path.GetDirectoryName((new Uri(assembly.CodeBase)).AbsolutePath) + Path.DirectorySeparatorChar.ToString();
        }
    }
}
