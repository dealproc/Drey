using Drey.Configuration.Infrastructure;

using Microsoft.AspNet.SignalR.Client;

using System;

namespace Drey.Configuration.ServiceModel
{
    public interface IReportPeriodically : IDisposable
    {
        void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy);
        void Stop();
    }
}
