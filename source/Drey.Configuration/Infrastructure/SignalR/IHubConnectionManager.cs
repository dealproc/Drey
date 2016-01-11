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
        /// <summary>
        /// Gets or sets the retry period (in milliseconds).
        /// </summary>
        /// <value>
        /// The retry period (in milliseconds).
        /// </value>
        int RetryPeriod { get; set; }

        /// <summary>
        /// Retrieves the State property from the managed HubConnection.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// Retrieves the ConnectionType property from the managed HubConnection.
        /// </summary>
        IClientTransport ConnectionType { get; }

        /// <summary>
        /// Facade for the HubConnection's Error event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<Exception> Error;

        /// <summary>
        /// Facade for the HubConnection's Received event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<string> Received;

        /// <summary>
        /// Facade for the HubConnection's Closed event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Closed;

        /// <summary>
        /// Facade for the HubConnection's Reconnecting event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Reconnecting;

        /// <summary>
        /// Facade for the HubConnection's Reconnected event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action Reconnected;

        /// <summary>
        /// Facade for the HubConnection's ConnectionSlow event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action ConnectionSlow;

        /// <summary>
        /// Facade for the HubConnection's StateChanged event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Proxy to SignalR event with same signature.")]
        event Action<StateChange> StateChanged;

        /// <summary>
        /// Allows the use of a client certificate by the hubconnection for client identification by the server.
        /// </summary>
        /// <param name="cert">The cert.</param>
        void UseClientCertificate(X509Certificate2 cert);

        /// <summary>
        /// Creates the hub proxy.
        /// </summary>
        /// <param name="hubName">Name of the hub.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">hubName</exception>
        IHubProxy CreateHubProxy(string hubName);

        /// <summary>
        /// Initializes the HubConnection and starts the connection with the server.
        /// </summary>
        Task Initialize();

        /// <summary>
        /// Shuts-down the managed HubConnection.
        /// </summary>
        void Stop();
    }
}