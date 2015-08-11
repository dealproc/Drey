using Nancy;

namespace Drey.Server.Modules.well_known
{
    public class PackagesModule : NancyModule
    {
        readonly Services.IPackageStore _packageStore;

        public PackagesModule(Services.IPackageStore packageStore) : base("~/.well-known/Packages")
        {
            _packageStore = packageStore;

            Get["/"] = _ => Response.AsJson(_packageStore.ListPackages());
            Get["/{packageId}"] = props => Response.AsJson(_packageStore.GetPackage((string)props.packageId));
        }
    }
}