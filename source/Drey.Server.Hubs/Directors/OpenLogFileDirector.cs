using Microsoft.AspNet.SignalR;

namespace Drey.Server.Directors
{
    public class OpenLogFileDirector : Director<DomainModel.Request<DomainModel.FileDownloadOptions>, DomainModel.Response<byte[]>>, IOpenLogFileDirector
    {
        public OpenLogFileDirector(IEventBus eventBus, IHubContext<Hubs.IRuntimeClient> runtimeClientContext, int timeoutInSeconds = 30)
            : base(eventBus, runtimeClientContext, timeoutInSeconds) { }

        /// <summary>
        /// Initiates a request to retrieve a file from a client for a consumer.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        public override void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginOpenLogFile(message);
        }
    }
}
