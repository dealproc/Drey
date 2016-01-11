using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IListLogsDirector : IHandle<DomainModel.Response<IEnumerable<string>>>
    {
        /// <summary>
        /// Initiates the consumer's request to a client to retrieve a list of logs.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        void Initiate(string clientId, DomainModel.Request<DomainModel.Empty> message);

        /// <summary>
        /// Gets the pending task, to hold the consumer's request until the client has a chance to respond.
        /// </summary>
        Task<DomainModel.Response<IEnumerable<string>>> PendingTask { get; }
    }
}
