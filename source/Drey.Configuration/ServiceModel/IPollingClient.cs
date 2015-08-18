using System;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    interface IPollingClient
    {
        void Start(CancellationToken ct);
    }
}