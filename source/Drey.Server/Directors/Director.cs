using Drey.Server.Logging;

using Microsoft.AspNet.SignalR;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace Drey.Server.Directors
{
    public abstract class Director<TRequest, TResponse> : IHandle<TResponse>, IDisposable
    {
        protected static ILog Log { get; private set; }

        bool _disposed = false;

        readonly IEventBus _eventBus;
        protected IHubContext<DomainModel.IRuntimeClient> RuntimeClientContext { get; private set; }
        
        readonly Timer _responseTimeout;
        readonly TaskCompletionSource<TResponse> _responseCompletion;
        readonly Stopwatch _stopWatch = new Stopwatch();

        public Director(IEventBus eventBus, IHubContext<DomainModel.IRuntimeClient> runtimeClientContext)
        {
            _eventBus = eventBus;
            RuntimeClientContext = runtimeClientContext;

            _responseCompletion = new TaskCompletionSource<TResponse>();
            _responseTimeout = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            _responseTimeout.Elapsed += responseTimeout_Elapsed;
        }
        static Director()
        {
            Log = LogProvider.GetCurrentClassLogger();
        }
        ~Director()
        {
            Dispose(false);
        }

        public Task<TResponse> PendingTask { get { return _responseCompletion.Task; } }

        /// <summary>
        /// Initiates the specified client identifier.
        /// </summary>
        /// <param name="clientId">This should be the username of the connected client.</param>
        /// <param name="message">The message.</param>
        public virtual void Initiate(string clientId, DomainModel.Request<TRequest> message)
        {
            Log.DebugFormat("Initiating {0} subscription to event bus.", this.GetType().Name);
            _eventBus.Subscribe(this, message.Token);

            Log.Trace("Enabling stopwatch and response timeout monitoring.");
            _stopWatch.Start();
            _responseTimeout.Start();
        }

        public void Handle(TResponse message)
        {
            _responseTimeout.Stop();
            _stopWatch.Stop();

            Log.DebugFormat("{director} succeeded in {milliseconds}ms.", this.GetType().Name, _stopWatch.ElapsedMilliseconds);

            _responseCompletion.SetResult(message);
        }

        private void responseTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            _responseTimeout.Stop();
            _stopWatch.Stop();

            _responseCompletion.SetException(new TimeoutException("Runtime client did not respond within the alloted time."));
        }

        public void Dispose()
        {
            Dispose(true);
            _disposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }
        
            if (_responseTimeout != null)
            {
                _responseTimeout.Dispose();
            }

            if (_eventBus != null)
            {
                Log.TraceFormat("{director} is unsubscribing from the message bus.", this.GetType().Name);
                _eventBus.Unsubscribe(this);
            }
        }
    }
}
