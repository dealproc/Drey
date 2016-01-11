using Microsoft.AspNet.SignalR;

using System.Collections.Generic;

namespace Drey.Server.Directors
{
    public class ListLogsDirector : Director<DomainModel.Request<DomainModel.Empty>, DomainModel.Response<IEnumerable<string>>>, IListLogsDirector
    {
        public ListLogsDirector(IEventBus eventBus, IHubContext<Hubs.IRuntimeClient> runtimeClientContext, int timeoutInSeconds = 30)
            : base(eventBus, runtimeClientContext, timeoutInSeconds) { }

        /// <summary>
        /// Initiates the consumer's request to a client to retrieve a list of logs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        public override void Initiate(string clientId, Drey.DomainModel.Request<Drey.DomainModel.Empty> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginListLogFiles(message);
        }
    }
}
