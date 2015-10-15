using Microsoft.AspNet.SignalR;

namespace Drey.Server.Directors
{
    public class OpenLogFileDirector : Director<DomainModel.Request<DomainModel.FileDownloadOptions>, DomainModel.Response<byte[]>>, IOpenLogFileDirector
    {
        public OpenLogFileDirector(IEventBus eventBus, IHubContext<DomainModel.IRuntimeClient> runtimeClientContext)
            : base(eventBus, runtimeClientContext) { }

        public override void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message)
        {
            base.Initiate(clientId, message);
            RuntimeClientContext.Clients.Group(clientId).BeginOpenLogFile(message);
        }
    }
}
