using Drey.Server.Extensions;

using Nancy;
using Nancy.ModelBinding;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    public class ListLogsModule : NancyModule
    {
        readonly Directors.IListLogsDirector _director;

        public ListLogsModule(Directors.IListLogsDirector director) : base("/runtime")
        {
            _director = director;

            Post["/Logs/ListFiles/{clientId}", runAsync: true] = ListLogFilesAsync;
        }

        private Task<dynamic> ListLogFilesAsync(dynamic arg, CancellationToken ct)
        {
            var model = this.Bind<DomainModel.Request<DomainModel.Empty>>();
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                var badRequest = new Nancy.Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Requests require a token querystring value to process." };
                return Task.FromResult(badRequest).AsDynamicTask();
            }

            _director.Initiate(arg.clientId, model);
            return _director.PendingTask.AsDynamicTask();
        }
    }

    public class OpenLogFileModule : NancyModule
    {
        readonly Directors.IOpenLogFileDirector _director;

        public OpenLogFileModule(Directors.IOpenLogFileDirector director) : base("/runtime")
        {
            _director = director;

            Post["/Logs/OpenLog/{clientId}", runAsync: true] = OpenLogFileAsync;
        }

        private Task<dynamic> OpenLogFileAsync(dynamic arg, CancellationToken ct)
        {
            var model = this.Bind<DomainModel.Request<DomainModel.FileDownloadOptions>>();
            if (string.IsNullOrWhiteSpace(model.Token))
            {
                var badRequest = new Nancy.Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "Requests require a token querystring value to process." };
                return Task.FromResult(badRequest).AsDynamicTask();
            }

            _director.Initiate(arg.clientId, model);
            return _director.PendingTask.AsDynamicTask();
        }
    }
}
