using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    public class PackageModule : NancyModule
    {
        public PackageModule() : base("/Package")
        {
            Post["/Upload/Complete"] = _ => new NotImplementedException();
            Post["/Upload"] = _ => new NotImplementedException();
            Post["/Register"] = _ => new NotImplementedException();
            Get["/{packageId}/{version}/download"] = _ => new NotImplementedException();
            Delete["/{packageId}"] = _ => new NotImplementedException();
        }
    }
}