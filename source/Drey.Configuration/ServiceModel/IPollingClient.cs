using System;
using System.Threading;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPollingClient
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Starts the specified ct.
        /// </summary>
        /// <param name="ct">The ct.</param>
        void Start(CancellationToken ct);
    }
}