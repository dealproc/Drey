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
            var path = Path.GetDirectoryName((new Uri(assembly.CodeBase)).AbsolutePath) + Path.DirectorySeparatorChar.ToString();

            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // see if the root folder marker exists.  if not, append it.
                path = path.StartsWith(Path.DirectorySeparatorChar.ToString()) ? path : Path.DirectorySeparatorChar.ToString() + path;
            }

            return path;
        }
    }
}
