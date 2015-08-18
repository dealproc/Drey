using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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