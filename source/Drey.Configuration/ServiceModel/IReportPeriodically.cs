using Drey.Configuration.Infrastructure;

using Microsoft.AspNet.SignalR.Client;

using System;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Interface definition for tasks that routinely push information to the server.
    /// </summary>
    public interface IReportPeriodically : IDisposable
    {
        /// <summary>
        /// Starts the specified reporting service.
        /// </summary>
        /// <param name="hubConnectionManager">The hub connection manager.</param>
        /// <param name="runtimeHubProxy">The runtime hub proxy.</param>
        void Start(IHubConnectionManager hubConnectionManager, IHubProxy runtimeHubProxy);

        /// <summary>
        /// Stops the reporting service.
        /// </summary>
        void Stop();
    }
}
