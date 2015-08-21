using System.IO;
using System.Reflection;

namespace Drey.Utilities
{
    static class PathUtilities
    {
        /// <summary>
        /// Resolves the path.
        /// <remarks>There may be an alternative method to do this, using the inherit .net framework.  to be analyzed and discovered</remarks>
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="includePathSeparator">if set to <c>true</c> [include path separator].</param>
        /// <returns></returns>
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