using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Drey.Configuration.Repositories.OnDisk
{
    public class OnDiskPackageRepository : MarshalByRefObject, IPackageRepository
    {
        static Regex VERSION_PARSER = new Regex(@"(\d+).(\d+)(.(\d+)?)(.(\d+)?)", RegexOptions.Compiled);

        readonly Drey.Nut.INutConfiguration _configurationManager;

        public OnDiskPackageRepository(Drey.Nut.INutConfiguration configurationManager) : base()
        {
            _configurationManager = configurationManager;
        }

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

        public IEnumerable<DataModel.Package> GetPackages()
        {
            return All()
                .Select(x => new DataModel.Package { Id = x.Id, Title = x.Description, AutoUpdates = false })
                .Distinct();
        }

        public IEnumerable<DataModel.Release> GetReleases(string packageId)
        {
            return All().Where(r => r.Id == packageId);
        }

        public DataModel.Release Store(DataModel.Release r)
        {
            throw new NotSupportedException("On-Disk Package Repository is for development purposes only.");
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Delete(string packageId, string version)
        {
            throw new NotSupportedException("we do not delete packages from disc.");
        }
    }
}
