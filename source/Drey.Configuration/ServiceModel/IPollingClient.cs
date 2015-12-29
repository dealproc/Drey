using System;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    public interface IPollingClient
    {
        string Title { get; }
        /// <summary>
        /// Starts the specified ct.
        /// </summary>
        /// <param name="ct">The ct.</param>
        void Start(CancellationToken ct);
    }
}