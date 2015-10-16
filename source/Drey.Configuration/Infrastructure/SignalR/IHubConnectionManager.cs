using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Drey.Configuration.Infrastructure
{
    /// <summary>
    /// Monitors and heals a HubConnection upon failure
    /// </summary>
    public interface IHubConnectionManager : IDisposable
    {
        int RetryPeriod { get; set; }
        ConnectionState State { get; }
        IClientTransport ConnectionType { get; }
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<Exception> Error;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<string> Received;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Closed;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Reconnecting;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Reconnected;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action ConnectionSlow;
        
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<StateChange> StateChanged;
        
        void UseClientCertificate(X509Certificate2 cert);
        IHubProxy CreateHubProxy(string hubName);
        Task Initialize();
        void Stop();
    }
}