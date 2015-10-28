using Drey.Server.Logging;

using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server.Controllers
{
    public class RecycleClientController : ApiController
    {
        static readonly ILog _log = LogProvider.For<RecycleClientController>();

        readonly Directors.IRecycleClientDirector _director;

        public RecycleClientController(Directors.IRecycleClientDirector director)
        {
            _director = director;
        }

        [HttpPost, Route("/runtime/RecycleClient/{clientId}")]
        public async Task<IHttpActionResult> Post([FromUri] string clientId, [FromUri] string token)
        {
            var model = new DomainModel.Request<DomainModel.Empty> { Token = token, Message = new DomainModel.Empty() };

            if (string.IsNullOrWhiteSpace(model.Token))
            {
                _log.DebugFormat("Token was not on uri. '{uri}'", this.ActionContext.Request.RequestUri);
                return BadRequest("Requests require a token querystring value to process.");
            }

            _director.Initiate(clientId, model);
            var taskResult = await _director.PendingTask;

            _log.Debug("Client should be recycling.");
            return Ok(taskResult);
        }
    }
}
