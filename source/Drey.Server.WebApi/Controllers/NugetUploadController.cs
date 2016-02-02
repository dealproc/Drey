using Drey.Server.Logging;
using Drey.Server.Services;

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server.Controllers {
	[RoutePrefix("api/v2/package")]
    public class NugetUploadController : ApiController
    {
        static readonly ILog _log = LogProvider.For<NugetUploadController>();

        readonly IPackageService _packageService;
        readonly INugetApiClaimsValidator _claimsValidator;

        public NugetUploadController(IPackageService packageService, INugetApiClaimsValidator claimsValidator)
        {
            _packageService = packageService;
            _claimsValidator = claimsValidator;
        }

        [HttpPut, Route("")]
        public async Task<IHttpActionResult> ParseAndStorePackageAsync()
        {
            if (User != null && User is ClaimsPrincipal)
            {
                if (!_claimsValidator.Validate(((ClaimsPrincipal)User).Claims.ToArray()))
                {
                    return BadRequest();
                }
            }

            var msProvider = await Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());

            if (!msProvider.Contents.Any())
            {
                return BadRequest("Missing File");
            }

            var stream = await msProvider.Contents.First().ReadAsStreamAsync();

            var release = await _packageService.SyndicateAsync(stream);

            return Created(string.Format(".well-known/releases/{0}/{1}", release.Id, release.Version), release);
        }

        [HttpDelete, Route("{id}/{version}")]
        public async Task<IHttpActionResult> DeleteAsync(string id, string version)
        {
            try
            {
                await _packageService.DeleteAsync(id, version);
                return Ok();
            }
            catch (Exception ex)
            {
                _log.FatalException("Unknown error occurred.", ex);
                return InternalServerError(ex);
            }
        }
    }
}
