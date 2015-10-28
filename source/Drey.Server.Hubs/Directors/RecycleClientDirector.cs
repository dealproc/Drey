using Microsoft.AspNet.SignalR;

namespace Drey.Server.Directors
{
    public class RecycleClientDirector : Director<DomainModel.Request<DomainModel.Empty>, DomainModel.Response<DomainModel.Empty>>, IRecycleClientDirector
    {
        public RecycleClientDirector(IEventBus eventBus, IHubContext<Hubs.IRuntimeClient> runtimeClientContext, int timeoutInSeconds = 30)
            : base(eventBus, runtimeClientContext, timeoutInSeconds) { }

        public override void Initiate(string clientId, Drey.DomainModel.Request<Drey.DomainModel.Empty> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginRecycleClient(message);
        }
    }
}
