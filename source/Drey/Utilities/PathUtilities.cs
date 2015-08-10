using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Utilities
{
    static class PathUtilities
    {
        public static string ResolvePath(string relativePath)
        {
            var respondWith = relativePath;

            if (respondWith.StartsWith("~/"))
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Remove(0, 8)) + DreyConstants.PathSeparator;
                respondWith = respondWith.Replace("~/", dir);
            }

            if (!respondWith.EndsWith(DreyConstants.PathSeparator))
            {
                respondWith += DreyConstants.PathSeparator;
            }

            respondWith = respondWith.Replace("/", DreyConstants.PathSeparator);

            return respondWith;
        }
    }
}
