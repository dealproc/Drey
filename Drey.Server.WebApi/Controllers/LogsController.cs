using Drey.Server.Logging;
using System.Threading.Tasks;
using System.Web.Http;

namespace Drey.Server.Controllers
{
    public class ListLogsController : ApiController
    {
        static readonly ILog _log = LogProvider.For<ListLogsController>();
        readonly Directors.IListLogsDirector _director;

        public ListLogsController(Directors.IListLogsDirector director)
        {
            _director = director;
        }

        [Route("/runtime/Logs/ListFiles/{clientId}")]
        public async Task<IHttpActionResult> Post([FromUri]string clientId, [FromUri]string token)
        {
            var model = new DomainModel.Request<DomainModel.Empty> { Token = token, Message = new DomainModel.Empty() };

            if (string.IsNullOrWhiteSpace(model.Token))
            {
                _log.DebugFormat("Token was not on uri. '{uri}'", this.ActionContext.Request.RequestUri);
                return BadRequest("Requests require a token querystring value to process.");
            }

            _director.Initiate(clientId, model);
            var taskResult = await _director.PendingTask;

            _log.Debug("Returning list of files from client.");
            return Ok(taskResult);
        }
    }
    public class OpenLogFileController : ApiController
    {
        static readonly ILog _log = LogProvider.For<OpenLogFileController>();
        readonly Directors.IOpenLogFileDirector _director;

        public OpenLogFileController(Directors.IOpenLogFileDirector director)
        {
            _director = director;
        }

        [Route("/runtime/Logs/OpenLog/{clientId}")]
        public async Task<IHttpActionResult> Post([FromUri]string clientId, [FromUri]string token, [FromBody]DomainModel.Request<DomainModel.FileDownloadOptions> model)
        {
            model.Token = token;

            if (string.IsNullOrWhiteSpace(model.Token))
            {
                _log.DebugFormat("Token was not on uri. '{uri}'", this.ActionContext.Request.RequestUri);
                return BadRequest("Requests require a token querystring value to process.");
            }

            _director.Initiate(clientId, model);
            var taskResult = await _director.PendingTask;

            _log.Debug("Returning file from client.");
            return Ok(taskResult);
        }
    }
}
