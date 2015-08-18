using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    class PollingClientCollection : ConcurrentBag<IPollingClient>, IDisposable
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        public new void Add(IPollingClient client)
        {
            base.Add(client);

            client.Start(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}