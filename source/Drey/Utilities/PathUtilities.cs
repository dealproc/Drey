using System.IO;
using System.Reflection;

namespace Drey.Utilities
{
    static class PathUtilities
    {
        public static string ResolvePath(string relativePath, bool includePathSeparator = true)
        {
            var respondWith = relativePath;

            if (respondWith.StartsWith("~/"))
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Remove(0, 8)) + DreyConstants.PathSeparator;
                respondWith = respondWith.Replace("~/", dir);
            }

            if (!respondWith.EndsWith(DreyConstants.PathSeparator) && includePathSeparator)
            {
                respondWith += DreyConstants.PathSeparator;
            }

            respondWith = respondWith.Replace("/", DreyConstants.PathSeparator);

            return respondWith;
        }
    }
}
