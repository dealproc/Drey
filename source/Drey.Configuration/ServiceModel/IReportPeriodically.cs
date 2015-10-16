using Drey.Configuration.Infrastructure;
using Microsoft.AspNet.SignalR.Client;

namespace Drey.Configuration.ServiceModel
{
    interface IReportPeriodically
    {
        void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy);
    }
}
