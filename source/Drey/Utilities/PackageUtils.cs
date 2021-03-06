﻿using System;
using System.IO;
using System.Linq;

namespace Drey.Utilities
{
    public static class PackageUtils
    {
        /// <summary>
        /// Discovers the latest package from the hoarde and returns its path.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="hordeDirectory">The horde directory.</param>
        /// <param name="specificVersion">The specific version.</param>
        /// <returns></returns>
        public static string DiscoverPackage(string packageId, string hordeDirectory, string specificVersion = "Latest")
        {
            var configPath = PathUtilities.MapPath(hordeDirectory);

            var versionFolders = Directory.GetDirectories(configPath, packageId + "*").Select(dir => (new DirectoryInfo(dir)).Name);
            Version latestVersion;

            if (specificVersion.Equals("Latest"))
            {
                var versions = versionFolders.Select(ver => new Version(ver.ToLower().Replace(packageId.ToLower() + ".", string.Empty)));
                if (!versions.Any()) { return string.Empty; }

                latestVersion = versions.OrderByDescending(x => x).First();
            }
            else
            {
                latestVersion = new Version(specificVersion);
            }

            return Path.Combine(configPath, string.Format("{0}.{1}", packageId, latestVersion.ToString()));
        }
    }
}