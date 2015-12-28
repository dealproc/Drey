using Drey.Server.Logging;
using Drey.Server.Services;

using Nancy;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    public class NugetUploadModule : NancyModule
    {
        static readonly ILog _Log = LogProvider.For<NugetUploadModule>();

        readonly IPackageService _packageService;

        public NugetUploadModule(IPackageService packageService) : base("/api/v2/package")
        {
            _packageService = packageService;

            Put["/", runAsync: true] = ParseAndStorePackage;
            Delete["/{id}/{version}", runAsync: true] = DeleteReleaseAsync;
        
            this.Before.AddItemToEndOfPipeline(ctx => { _Log.Trace(ctx.Request.Url); return (Response)null; });
        }

        private async Task<dynamic> DeleteReleaseAsync(dynamic args, CancellationToken ct)
        {
            string id;
            string version;

            try
            {
                id = (string)args.id;
                version = (string)args.version;

                await _packageService.DeleteAsync(id, version);

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _Log.FatalException("Unknown error occurred.", ex);
                return ((Response)"An unknown error occurred.  We will be investigating shortly.").StatusCode = HttpStatusCode.InternalServerError;
            }
        }

        private async Task<dynamic> ParseAndStorePackage(dynamic _, CancellationToken ct)
        {
            if (!Request.Files.Any())
            {
                return ((Response)"Missing file").StatusCode = HttpStatusCode.BadRequest;
            }

            _Log.Info("Received a file to be parsed and stored.");

            try
            {
                await _packageService.SyndicateAsync(Request.Files.First().Value);
            }
            catch (Exception ex)
            {
                _Log.ErrorException("Could not syndicate package.", ex);
                return HttpStatusCode.BadRequest;
            }
            return HttpStatusCode.Created;
        }
    }
}