using System;
using System.IO;
using System.Linq;

namespace Drey.Utilities
{
    public static class PackageUtils
    {
        public static string DiscoverPackage(string packageId, string hordeDirectory)
        {
            var configPath = Utilities.PathUtilities.ResolvePath(hordeDirectory);

            var versionFolders = Directory.GetDirectories(hordeDirectory, packageId + "*").Select(dir => (new DirectoryInfo(dir)).Name);
            var versions = versionFolders.Select(ver => new Version(ver.Replace(packageId + "-", string.Empty)));
            var latestVersion = versions.OrderByDescending(x => x).First();

            return Path.Combine(hordeDirectory, string.Format("{0}-{1}", packageId, latestVersion.ToString()));
        }
    }
}