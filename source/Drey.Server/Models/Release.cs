using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Server.Models
{
    public class Release
    {
        public string SHA1 { get; set; }
        public string Filename { get; set; }
        public long Filesize { get; set; }
    }
}
