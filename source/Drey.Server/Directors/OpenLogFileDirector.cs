using Microsoft.AspNet.SignalR;

namespace Drey.Server.Directors
{
    internal class OpenLogFileDirector : Director<DomainModel.FileDownloadOptions, byte[]>, IOpenLogFileDirector
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
