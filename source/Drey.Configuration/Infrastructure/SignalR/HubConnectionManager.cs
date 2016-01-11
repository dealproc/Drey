using Drey.Logging;

using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Drey.Configuration.Infrastructure
{
    /// <summary>
    /// Monitors and heals a HubConnection upon failure
    /// </summary>
    public class HubConnectionManager : IHubConnectionManager, IDisposable
    {
        static readonly ILog _log = LogProvider.For<HubConnectionManager>();

        private HubConnection _hubConnection;

        private int _retryPeriod = 10000;
        private bool _disposed = false;

        /// <summary>
        /// Facade for the HubConnection's Error event.
        /// </summary>
        public event Action<Exception> Error;

        /// <summary>
        /// Facade for the HubConnection's Received event.
        /// </summary>
        public event Action<string> Received;
        
        /// <summary>
        /// Facade for the HubConnection's Closed event.
        /// </summary>
        public event Action Closed;
        
        /// <summary>
        /// Facade for the HubConnection's Reconnecting event.
        /// </summary>
        public event Action Reconnecting;
        
        /// <summary>
        /// Facade for the HubConnection's Reconnected event.
        /// </summary>
        public event Action Reconnected;
        
        /// <summary>
        /// Facade for the HubConnection's ConnectionSlow event.
        /// </summary>
        public event Action ConnectionSlow;
        
        /// <summary>
        /// Facade for the HubConnection's StateChanged event.
        /// </summary>
        public event Action<StateChange> StateChanged;

        /// <summary>
        /// Gets or sets the retry period (in milliseconds).
        /// </summary>
        /// <value>
        /// The retry period (in milliseconds).
        /// </value>
        public int RetryPeriod
        {
            get { return _retryPeriod; }
            set
            {
                //Sir, you don't want a negative retry period
                if (RetryPeriod <= 0)
                {
                    return;
                }

                _retryPeriod = value;
            }
        }

        /// <summary>
        /// Retrieves the State property from the managed HubConnection.
        /// </summary>
        public ConnectionState State
        {
            get { return _hubConnection.State; }
        }

        /// <summary>
        /// Retrieves the ConnectionType property from the managed HubConnection.
        /// </summary>
        public IClientTransport ConnectionType
        {
            get { return _hubConnection.Transport; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HubConnectionManager"/> class.
        /// <remarks>This creates a hubconnection with default options, and assigns the provided url.</remarks>
        /// </summary>
        /// <param name="url">The URL.</param>
        private HubConnectionManager(string url)
        {
            _hubConnection = new HubConnection(url);
            _hubConnection.TraceWriter = new LibLogTraceWriter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HubConnectionManager"/> class.
        /// <remarks>This allows the capability of providing a custom configured HubConnection to be managed.</remarks>
        /// </summary>
        /// <param name="hubConnection">The hub connection.</param>
        private HubConnectionManager(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _hubConnection.TraceWriter = new LibLogTraceWriter();
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="HubConnectionManager"/> class.
        /// </summary>
        ~HubConnectionManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the hub connection manager.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static IHubConnectionManager GetHubConnectionManager(string url)
        {
            IHubConnectionManager connectionManager = new HubConnectionManager(url);
            return connectionManager;
        }

        /// <summary>
        /// Gets the hub connection manager.
        /// </summary>
        /// <param name="hubConnection">The hub connection.</param>
        /// <returns></returns>
        public static IHubConnectionManager GetHubConnectionManager(HubConnection hubConnection)
        {
            IHubConnectionManager connectionManager = new HubConnectionManager(hubConnection);
            return connectionManager;
        }

        /// <summary>
        /// Allows the use of a client certificate by the hubconnection for client identification by the server.
        /// </summary>
        /// <param name="cert">The cert.</param>
        public void UseClientCertificate(X509Certificate2 cert)
        {
            _hubConnection.AddClientCertificate(cert);
        }

        /// <summary>
        /// Creates the hub proxy.
        /// </summary>
        /// <param name="hubName">Name of the hub.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">hubName</exception>
        public IHubProxy CreateHubProxy(string hubName)
        {
            if (string.IsNullOrEmpty(hubName))
            {
                throw new ArgumentNullException("hubName");
            }

            return _hubConnection.CreateHubProxy(hubName);
        }

        /// <summary>
        /// Initializes the HubConnection and starts the connection with the server.
        /// </summary>
        public async Task Initialize()
        {
            _hubConnection.Received += OnReceived;
            _hubConnection.Closed += OnClosed;
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;
            _hubConnection.ConnectionSlow += OnConnectionSlow;
            _hubConnection.Error += OnError;
            _hubConnection.StateChanged += OnStateChanged;

            try
            {
                await _hubConnection.Start();
            }
            catch (Exception)
            {
                // squelch issue(s);
            }
        }

        /// <summary>
        /// Shuts-down the managed HubConnection.
        /// </summary>
        public void Stop()
        {
            _hubConnection.Received -= OnReceived;
            _hubConnection.Closed -= OnClosed;
            _hubConnection.Reconnecting -= OnReconnecting;
            _hubConnection.Reconnected -= OnReconnected;
            _hubConnection.ConnectionSlow -= OnConnectionSlow;
            _hubConnection.Error -= OnError;
            _hubConnection.StateChanged -= OnStateChanged;

            _hubConnection.Stop();
        }

        private void OnReceived(string data)
        {
            if (Received != null)
            {
                Received(data);
            }
        }

        private async void OnClosed()
        {
            if (Closed != null)
            {
                Closed();
            }
            await RetryConnection();
        }

        private async Task RetryConnection()
        {
            await Task.Delay(RetryPeriod);
            try
            {
                await _hubConnection.Start();
            }
            catch (Exception exc)
            {
                _log.ErrorException(exc.Message, exc);
            }
        }

        private void OnReconnecting()
        {
            if (Reconnecting != null)
            {
                Reconnecting();
            }
        }

        private void OnReconnected()
        {
            if (Reconnected != null)
            {
                Reconnected();
            }
        }

        private void OnConnectionSlow()
        {
            if (ConnectionSlow != null)
            {
                ConnectionSlow();
            }
        }

        private void OnError(Exception error)
        {
            if (Error != null)
            {
                _log.ErrorException("", error);
                Error(error);
            }
        }
        
        private void OnStateChanged(StateChange stateChange)
        {
            if (StateChanged != null)
            {
                StateChanged(stateChange);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            if (_hubConnection != null)
            {
                _log.Debug("Hub connection is being disposed.");
                _hubConnection.Dispose();
                _hubConnection = null;
                _log.Info("Hub connection has been disposed.");
            }
            else
            {
                _log.Debug("Hub connection was not setup.");
            }

            _disposed = true;
        }
    }
}