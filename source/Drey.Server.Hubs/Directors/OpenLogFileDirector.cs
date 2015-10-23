using Microsoft.AspNet.SignalR;

namespace Drey.Server.Directors
{
    public class OpenLogFileDirector : Director<DomainModel.Request<DomainModel.FileDownloadOptions>, DomainModel.Response<byte[]>>, IOpenLogFileDirector
    {
        public OpenLogFileDirector(IEventBus eventBus, IHubContext<Hubs.IRuntimeClient> runtimeClientContext, int timeoutInSeconds = 30)
            : base(eventBus, runtimeClientContext, timeoutInSeconds) { }

        public override void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginOpenLogFile(message);
        }
    }
}
