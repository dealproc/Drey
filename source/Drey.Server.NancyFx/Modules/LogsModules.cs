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

        public ListLogsModule(Directors.IListLogsDirector director) : base("/runtime")
        {
            _director = director;

            Post["/Logs/ListFiles/{clientId}", runAsync: true] = ListLogFilesAsync;
        }

        private Task<dynamic> ListLogFilesAsync(dynamic arg, CancellationToken ct)
        {
            _log.Info("Attempting to list client logs.");
            var model = this.Bind<DomainModel.Request<DomainModel.Empty>>();

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

        public OpenLogFileModule(Directors.IOpenLogFileDirector director) : base("/runtime")
        {
            _director = director;

            Post["/Logs/OpenLog/{clientId}", runAsync: true] = OpenLogFileAsync;
        }

        private Task<dynamic> OpenLogFileAsync(dynamic arg, CancellationToken ct)
        {
            _log.Info("Attempting to open a log file from a client.");
            var model = this.Bind<DomainModel.Request<DomainModel.FileDownloadOptions>>();

            _log.DebugFormat("Client id: {clientId} | token {token}", (string)arg.clientId, model.Token);

            if (string.IsNullOrWhiteSpace(model.Token))
            {
                _log.Debug("Token was not present in request.");
                var badRequest = new Nancy.Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Requests require a token querystring value to process." };
                return Task.FromResult(badRequest).AsDynamicTask();
            }

            _log.Info("Requesting log file from client.");
            _director.Initiate(arg.clientId, model);
            return _director.PendingTask.AsDynamicTask();
        }
    }
}
