using Microsoft.AspNet.SignalR.Client;

namespace Drey.Configuration.ServiceModel
{
    public interface IRemoteInvocationService
    {
        void SubscribeToEvents(IHubProxy runtimeHubProxy);
    }
    public interface IRemoteInvocationService<TRequest, TResponse> : IRemoteInvocationService
        where TRequest : DomainModel.Request
        where TResponse : DomainModel.Response
    {
    }
}
