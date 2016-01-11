using Drey.Extensions;
using Drey.Logging;

using System.IO;
using System.Reflection;

namespace Drey.Utilities
{
    /// <summary>
    /// Shared utilities for working with paths.
    /// </summary>
    public static class PathUtilities
    {
        static ILog _log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Resolves the path.
        /// <remarks>There may be an alternative method to do this, using the inherit .net framework.  to be analyzed and discovered</remarks>
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="includePathSeparator">if set to <c>true</c> [include path separator].</param>
        /// <returns>A physical path, given a logical path input.</returns>
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

            if (
                (System.Environment.OSVersion.Platform == System.PlatformID.Unix || System.Environment.OSVersion.Platform == System.PlatformID.MacOSX)
                &&
                (!respondWith.StartsWith(Path.DirectorySeparatorChar.ToString()))
               )
            {
                _log.Debug("Detected a mac/unix environment.");
                respondWith = Path.DirectorySeparatorChar.ToString() + respondWith;
            }

            return respondWith.NormalizePathSeparator();
        }

        /// <summary>
        /// Normalizes the path separator in a filesystem path, for the given operating system.
        /// </summary>
        /// <param name="inPath">The in path.</param>
        /// <returns>A normalized path for the host operating system.</returns>
        public static string NormalizePathSeparator(this string inPath)
        {
            return inPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}