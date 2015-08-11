using Nancy;
using System;

namespace Drey.Server.Modules
{
    public class PackageModule : NancyModule
    {
        readonly Services.IPackageStore _packageStore;
        public PackageModule(Services.IPackageStore packageStore) : base("/Package")
        {
            _packageStore = packageStore;

            Post["/Push"] = _ => new NotImplementedException();
            Get["/{packageId}/{version}/download"] = _ => new NotImplementedException();
            Delete["/{packageId}"] = _ => new NotImplementedException();
        }
    }
}