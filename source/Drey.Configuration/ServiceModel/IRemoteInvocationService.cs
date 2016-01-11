using Microsoft.AspNet.SignalR.Client;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Defines the basis for a remote-invoked service (signalr push event).
    /// <remarks>DO NOT INHERIT FROM THIS.  THIS IS ONLY USEFUL FOR SERVICE LOCATION.</remarks>
    /// </summary>
    public interface IRemoteInvocationService
    {
        void SubscribeToEvents(IHubProxy runtimeHubProxy);
    }

    /// <summary>
    /// Defines the basis for a remote-invoked service (signalr push event).
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRemoteInvocationService<TRequest, TResponse> : IRemoteInvocationService
        where TRequest : DomainModel.Request
        where TResponse : DomainModel.Response
    {
    }
}
