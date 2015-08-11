using System;
using System.Collections.Generic;

namespace Drey.Server.Models
{
    public class PackageDetails
    {
        public string PackageId { get; set; }
        public string PackageName { get; set; }
        public List<ReleasePmo> Releases { get; set; }

        public PackageDetails()
        {
            Releases = new List<ReleasePmo>();
        }

        public class ReleasePmo
        {
            public string Version { get; set; }
            public DateTime ReleasedOn { get; set; }
            public Uri DownloadUri { get; set; }
        }
    }
}