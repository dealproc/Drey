using Drey.Server.Logging;
using Drey.Server.Services;

using Nancy;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    /// <summary>
    /// Mimic's the Nuget API for syndication of packages
    /// </summary>
    public class NugetUploadModule : NancyModule
    {
        static readonly ILog _Log = LogProvider.For<NugetUploadModule>();

        readonly IPackageService _packageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NugetUploadModule"/> class.
        /// </summary>
        /// <param name="packageService">The package service.</param>
        public NugetUploadModule(IPackageService packageService)
            : base("/api/v2/package")
        {
            _packageService = packageService;

            Put["/", runAsync: true] = ParseAndStorePackage;
            Delete["/{id}/{version}", runAsync: true] = DeleteReleaseAsync;
        
            this.Before.AddItemToEndOfPipeline(ctx => { _Log.Trace(ctx.Request.Url); return (Response)null; });
        }

        /// <summary>
        /// Deletes a release from the package store.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Receives a nuget package; parses it, and stores the package to a persistent storage for later retrieval.
        /// </summary>
        /// <param name="_">The _.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
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