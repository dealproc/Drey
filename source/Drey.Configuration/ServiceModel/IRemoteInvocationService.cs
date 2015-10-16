using Microsoft.AspNet.SignalR.Client;

namespace Drey.Configuration.ServiceModel
{
    interface IRemoteInvocationService<TRequest, TResponse>
        where TRequest : DomainModel.Request
        where TResponse : DomainModel.Response
    {
        void SubscribeToEvents(IHubProxy runtimeHubProxy);
    }
}
