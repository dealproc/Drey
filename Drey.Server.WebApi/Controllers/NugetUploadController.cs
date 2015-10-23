using Drey.Server.Logging;
using Drey.Server.Services;

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server.Controllers
{
    [RoutePrefix("api/v2/package")]
    public class NugetUploadController : ApiController
    {
        static readonly ILog _log = LogProvider.For<NugetUploadController>();

        readonly IPackageService _packageService;

        public NugetUploadController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPut, Route("")]
        public async Task<IHttpActionResult> ParseAndStorePackageAsync()
        {
            var msProvider = await Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider());

            if (!msProvider.Contents.Any())
            {
                return BadRequest("Missing File");
            }

            var stream = await msProvider.Contents.First().ReadAsStreamAsync();

            var release = await _packageService.SyndicateAsync(stream);

            return Created(string.Format(".well-known/releases/{0}/{1}", release.Id, release.Version), release);
        }
    }
}
