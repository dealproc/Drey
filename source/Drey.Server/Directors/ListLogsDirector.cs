using Microsoft.AspNet.SignalR;

using System.Collections.Generic;

namespace Drey.Server.Directors
{
    public class ListLogsDirector : Director<DomainModel.Request<DomainModel.Empty>, DomainModel.Response<IEnumerable<string>>>, IListLogsDirector
    {
        public ListLogsDirector(IEventBus eventBus, IHubContext<DomainModel.IRuntimeClient> runtimeClientContext, int timeoutInSeconds = 30)
            : base(eventBus, runtimeClientContext, timeoutInSeconds) { }

        public override void Initiate(string clientId, Drey.DomainModel.Request<Drey.DomainModel.Empty> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginListLogFiles(message);
        }
    }
}
