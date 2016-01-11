using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IRecycleClientDirector : IHandle<DomainModel.Response<DomainModel.Empty>>
    {
        /// <summary>
        /// Initiates a command to recycle a client's packages.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="message">The message.</param>
        void Initiate(string clientId, DomainModel.Request<DomainModel.Empty> message);
        
        /// <summary>
        /// Gets the pending task, to hold the consumer's request until the client has a chance to respond.
        /// </summary>
        Task<DomainModel.Response<DomainModel.Empty>> PendingTask { get; }
    }
}
