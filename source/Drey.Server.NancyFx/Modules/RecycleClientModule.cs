using Drey.Server.Extensions;

using Nancy;
using Nancy.ModelBinding;

using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules
{
    public class RecycleClientModule : NancyModule
    {
        readonly Directors.IRecycleClientDirector _director;

        public RecycleClientModule(Directors.IRecycleClientDirector director) : base("/runtime")
        {
            _director = director;

            Post["/RecycleClient/{clientId}", runAsync: true] = RecycleClientAsync;
        }

        private Task<dynamic> RecycleClientAsync(dynamic arg, CancellationToken ct)
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
}
