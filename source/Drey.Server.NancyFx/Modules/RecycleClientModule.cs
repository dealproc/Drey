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

        /// <summary>
        /// Initializes a new instance of the <see cref="RecycleClientModule"/> class.
        /// </summary>
        /// <param name="director">The director.</param>
        public RecycleClientModule(Directors.IRecycleClientDirector director)
            : base("/runtime")
        {
            _director = director;

            Post["/RecycleClient/{clientId}", runAsync: true] = RecycleClientAsync;
        }

        /// <summary>
        /// Issues a RecycleApp request to a client.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
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
