using Drey.Server.Logging;
using Drey.Server.Services;

using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server.Controllers
{
    public class ReleasesController : ApiController
    {
        static readonly ILog _log = LogProvider.For<ReleasesController>();

        readonly IPackageService _packageService;
        ClaimsPrincipal ClaimsUser { get { return this.User as ClaimsPrincipal; } }

        public ReleasesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet, Route(".well-known/packages")]
        public async Task<IHttpActionResult> GetSubscribedPackages()
        {
            return Ok(await _packageService.GetPackagesAsync(ClaimsUser));
        }

        [HttpGet, Route(".well-known/releases/{id}", Order = 2)]
        public async Task<IHttpActionResult> GetSubscribedPackagesAsync([FromUri]string id)
        {
            return Ok(await _packageService.GetPackagesAsync(ClaimsUser));
        }

        [HttpGet, Route(".well-known/releases/{id}/{version}", Order = 1)]
        public async Task<IHttpActionResult> DownloadReleaseAsync([FromUri]string id, [FromUri]string version)
        {
            var file = await _packageService.GetReleaseAsync(id, version, ClaimsUser);
            return new FileActionResult(file.FileContents, file.Filename, file.MimeType);
        }
    }
}
