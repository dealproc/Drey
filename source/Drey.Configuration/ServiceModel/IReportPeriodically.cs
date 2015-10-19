using Drey.Configuration.Infrastructure;
using Microsoft.AspNet.SignalR.Client;

namespace Drey.Configuration.ServiceModel
{
    public interface IReportPeriodically
    {
        void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy);
        void Stop();
    }
}
