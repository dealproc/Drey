using System.Collections.Generic;

namespace Drey.Server.Models
{
    public class Package
    {
        public string PackageId { get; set; }
        public IList<Release> Releases { get; set; }

        public Package()
        {
            Releases = new List<Release>();
        }
    }
}
