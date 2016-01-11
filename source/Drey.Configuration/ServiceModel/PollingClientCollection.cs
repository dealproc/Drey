using Drey.Logging;

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Manages the list of polling clients within the system (both for releases, and packages.)
    /// </summary>
    public class PollingClientCollection : ConcurrentBag<IPollingClient>, IDisposable
    {
        static ILog _log = LogProvider.For<PollingClientCollection>();

        bool _disposed = false;
        CancellationTokenSource _cts = new CancellationTokenSource();

        ~PollingClientCollection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Adds the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public new void Add(IPollingClient client)
        {
            _log.InfoFormat("Adding {title} polling client.", client.Title);
            base.Add(client);

            _log.DebugFormat("Starting {title} polling client.", client.Title);
            client.Start(_cts.Token);
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

            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            _disposed = true;
        }
    }
}