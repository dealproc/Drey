using Drey.Extensions;

using System.IO;
using System.Reflection;

namespace Drey.Utilities
{
    public static class PathUtilities
    {
        /// <summary>
        /// Resolves the path.
        /// <remarks>There may be an alternative method to do this, using the inherit .net framework.  to be analyzed and discovered</remarks>
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="includePathSeparator">if set to <c>true</c> [include path separator].</param>
        /// <returns></returns>
        public static string MapPath(string relativePath, bool includePathSeparator = true)
        {
            var respondWith = relativePath;

            if (respondWith.StartsWith(DreyConstants.RelativeUrlMarker))
            {

                var dir = Assembly.GetExecutingAssembly().GetDirectoryLocation();
                respondWith = respondWith.Replace(DreyConstants.RelativeUrlMarker, dir);
            }

            if (!respondWith.EndsWith(Path.DirectorySeparatorChar.ToString()) && includePathSeparator)
            {
                respondWith += Path.DirectorySeparatorChar.ToString();
            }

            return respondWith.NormalizePathSeparator();
        }

        public static string NormalizePathSeparator(this string inPath)
        {
            return inPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}