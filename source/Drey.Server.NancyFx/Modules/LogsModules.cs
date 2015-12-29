using Drey.Server.Extensions;
using Drey.Server.Logging;

using Nancy;
using Nancy.ModelBinding;

using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    public class ListLogsModule : NancyModule
    {
        static readonly ILog _log = LogProvider.For<ListLogsModule>();

        readonly Directors.IListLogsDirector _director;

        public ListLogsModule(Directors.IListLogsDirector director)
            : base("/runtime/Logs")
        {
            _director = director;

            Post["/ListFiles/{clientId}", runAsync: true] = ListLogFilesAsync;
        }

        private Task<dynamic> ListLogFilesAsync(dynamic arg, CancellationToken ct)
        {
            _log.Info("Attempting to list client logs.");
            var model = this.Bind<DomainModel.Request<DomainModel.Empty>>(); // TODO: dirty code... should be cleaned up to do the same as below.

            _log.DebugFormat("Client id: {clientId} | Token: {token}", (string)arg.clientId, model.Token);

            if (string.IsNullOrWhiteSpace(model.Token))
            {
                _log.Debug("Token was not present in request.");
                var badRequest = new Nancy.Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Requests require a token querystring value to process." };
                return Task.FromResult(badRequest).AsDynamicTask();
            }

            _log.Info("Requesting list of log files from client.");
            _director.Initiate(arg.clientId, model);
            return _director.PendingTask.AsDynamicTask();
        }
    }

    public class OpenLogFileModule : NancyModule
    {
        static readonly ILog _log = LogProvider.For<OpenLogFileModule>();

        readonly Directors.IOpenLogFileDirector _director;

        public OpenLogFileModule(Directors.IOpenLogFileDirector director)
            : base("/runtime/Logs")
        {
            _director = director;

            Post["/OpenLog/{clientId}", runAsync: true] = OpenLogFileAsync;
        }

        private Task<dynamic> OpenLogFileAsync(dynamic arg, CancellationToken ct)
        {
            _log.Info("Attempting to open a log file from a client.");

            string token = (string)this.Request.Query["token"];
            string clientId = (string)arg.clientId;

            var model = this.Bind<DomainModel.FileDownloadOptions>();

            if (string.IsNullOrWhiteSpace(token))
            {
                _log.Debug("Token was not present in request.");
                var badRequest = new Nancy.Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Requests require a token querystring value to process." };
                return Task.FromResult(badRequest).AsDynamicTask();
            }

            _log.DebugFormat("Client id: {clientId} | token {token}", clientId, token);

            _log.Info("Requesting log file from client.");
            _director.Initiate(clientId, new DomainModel.Request<DomainModel.FileDownloadOptions> { Token = token, Message = model });
            return _director.PendingTask.AsDynamicTask();
        }
    }
}
