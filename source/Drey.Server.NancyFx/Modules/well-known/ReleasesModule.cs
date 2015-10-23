using Drey.Server.Logging;
using Drey.Server.Services;

using Nancy;
using Nancy.Security;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules.well_known
{
    public class ReleasesModule : NancyModule
    {
        static readonly ILog _Log = LogProvider.For<ReleasesModule>();
        readonly IPackageService _packageService;

        public ReleasesModule(IPackageService packageService)
            : base("/.well-known")
        {
            _packageService = packageService;

            Get["/packages", runAsync: true] = GetSubscribedPackages;
            Get["/releases/{id}/{version}", runAsync: true] = DownloadReleaseAsync;
            Get["/releases/{id}", runAsync: true] = GetPackageReleasesAsync;

            this.Before.AddItemToEndOfPipeline(ctx => { _Log.Trace(ctx.Request.Url); return (Response)null; });
        }

        private async Task<dynamic> GetSubscribedPackages(dynamic args, CancellationToken ct)
        {
            return await _packageService.GetPackagesAsync(Context.GetMSOwinUser());
        }

        private async Task<dynamic> GetPackageReleasesAsync(dynamic args, CancellationToken ct)
        {
            try
            {
                _Log.Trace("Attempting to list releases for package id: " + (string)args.id);
                var releases = await _packageService.GetReleasesAsync((string)args.id, Context.GetMSOwinUser());

                if (releases.Any())
                {
                    return releases;
                }

                return ((Response)"No packages have been found.").StatusCode = HttpStatusCode.NotFound;
            }
            catch (KeyNotFoundException ex)
            {
                _Log.ErrorException("Listing releases for a package id failed.\t Package Id: {0}", ex, (string)args.id);
                return ((Response)ex.Message).StatusCode = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                _Log.FatalException("Unknown error occurred.", ex);
                return ((Response)"An unknown error occurred.  We will be investigating shortly.").StatusCode = HttpStatusCode.InternalServerError;
            }
        }

        private async Task<dynamic> DownloadReleaseAsync(dynamic args, CancellationToken ct)
        {
            string id = (string)args.id;
            string version = (string)args.version;

            try
            {
                var file = await _packageService.GetReleaseAsync(id, version, Context.GetMSOwinUser());
                var response = Response.FromStream(file.FileContents, file.MimeType);
                response.Headers.Add("Content-Disposition", "attachment; filename=\"" + file.Filename + "\"");
                return response;
            }
            catch (InvalidDataException ex)
            {
                _Log.InfoFormat("Could not retrieve package: {0} {1}", id, version);
                return ((Response)ex.Message).StatusCode = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                _Log.ErrorException("Retrieval of a release failed.\nPackage Id: {0} Version: {1}", ex, id, version);
                return ((Response)ex.Message).StatusCode = HttpStatusCode.InternalServerError;
            }
        }
    }
}