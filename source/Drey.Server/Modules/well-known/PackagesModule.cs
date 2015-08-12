using Drey.Server.Services;
using Nancy;

namespace Drey.Server.Modules.well_known
{
    public class PackagesModule : NancyModule
    {
        readonly IPackageService _packageService;

        public PackagesModule(IPackageService packageService) : base("/.well-known/packages")
        {
            _packageService = packageService;

            Get["/"] = _ => _packageService.ListPackages();
            Delete["/{packageId}"] = DeletePackage;
        }

        private dynamic DeletePackage(dynamic arg)
        {
            _packageService.DeletePackage((string)arg.packageId);
            return HttpStatusCode.OK;
        }
    }
}