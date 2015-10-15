using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    class PollingClientCollection : ConcurrentBag<IPollingClient>, IDisposable
    {
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
            base.Add(client);

            client.Start(_cts.Token);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            if (!disposing || _disposed) { return; }
        }
    }
}