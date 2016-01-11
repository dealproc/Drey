using System;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IOpenLogFileDirector : IHandle<DomainModel.Response<byte[]>>
    {
        /// <summary>
        /// Initiates a request to retrieve a file from a client for a consumer.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message);

        /// <summary>
        /// Gets the pending task, to hold the consumer's request until the client has a chance to respond.
        /// </summary>
        Task<DomainModel.Response<byte[]>> PendingTask { get; }
    }
}
