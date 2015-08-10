using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Server.Modules.well_known
{
    public class PackagesModule : NancyModule
    {
        public PackagesModule()
            : base("~/.well-known/Packages")
        {
            Get["/"] = _ => Response.AsJson(Enumerable.Empty<Models.NutInfoPmo>());
            Get["/{packageId}"] = props => Response.AsJson(new Models.NutDetailsPmo
            {
                PackageId = (string)props.packageId,
                PackageName = "Test Package",
                Releases = new List<Models.NutDetailsPmo.ReleasePmo>()
            });
        }
    }
}