using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Server.Models
{
    public class PackageInfo
    {
        public string PackageId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime ReleasedOn { get; set; }
        public Uri DownloadUri { get; set; }
    }
}
