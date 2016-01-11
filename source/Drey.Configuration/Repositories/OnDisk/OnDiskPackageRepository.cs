using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Drey.Configuration.Repositories.OnDisk
{
    /// <summary>
    /// Used when the system is in development mode, this infers stored data by parsing the folders in the ~/Hoarde folder on disc to 
    /// present a somewhat realistic representation of what would be stored for each package.
    /// </summary>
    public class OnDiskPackageRepository : MarshalByRefObject, IPackageRepository
    {
        static Regex VERSION_PARSER = new Regex(@"(\d+).(\d+)(.(\d+)?)(.(\d+)?)", RegexOptions.Compiled);

        readonly Drey.Nut.INutConfiguration _configurationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDiskPackageRepository"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public OnDiskPackageRepository(Drey.Nut.INutConfiguration configurationManager) : base()
        {
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.Release> All()
        {
            DirectoryInfo dir = new DirectoryInfo(Drey.Utilities.PathUtilities.MapPath(_configurationManager.HoardeBaseDirectory));
            return dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Select(x =>
            {
                string version = "";
                var versionMatches = VERSION_PARSER.Match(x.Name);

                if (versionMatches.Success)
                {
                    var matchedVersion = versionMatches.Groups[0];
                    version = matchedVersion.Value;
                }

                return new DataModel.Release
                {
                    CreatedOn = DateTime.Now,
                    Description = x.Name,
                    Id = x.Name.Replace(version, string.Empty).Trim('.'),
                    Version = version.Trim('.'),
                    UpdatedOn = DateTime.Now,
                    Title = x.Name,
                    Summary = string.Empty,
                    IconUrl = string.Empty,
                    Listed = true,
                    Published = DateTime.Now,
                    ReleaseNotes = string.Empty,
                    SHA1 = string.Empty,
                    Tags = string.Empty
                };
            })
            .Where(r => !r.Id.Equals(DreyConstants.ConfigurationPackageName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataModel.Package> GetPackages()
        {
            return All()
                .Select(x => new DataModel.Package { Id = x.Id, Title = x.Description, AutoUpdates = false })
                .Distinct();
        }

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return All().Where(r => r.Id == packageId);
        }

        /// <summary>
        /// Stores the specified r.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">On-Disk Package Repository is for development purposes only.</exception>
        public DataModel.Release Store(DataModel.Release r)
        {
            throw new NotSupportedException("On-Disk Package Repository is for development purposes only.");
        }

        /// <summary>
        /// Deletes the specified package identifier.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="System.NotSupportedException">we do not delete packages from disc.</exception>
        public void Delete(string packageId, string version)
        {
            throw new NotSupportedException("we do not delete packages from disc.");
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
