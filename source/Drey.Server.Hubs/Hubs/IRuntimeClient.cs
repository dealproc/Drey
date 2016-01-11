using Drey.DomainModel;
namespace Drey.Server.Hubs
{
    /// <summary>
    /// An interface to aid in unit testing.  This represents the calls we expect to be able to have responded to/by the client.
    /// </summary>
    public interface IRuntimeClient
    {
        /// <summary>
        /// Kick-off the process to list the available log files for viewing by a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        void BeginListLogFiles(Request<Empty> request);
        
        /// <summary>
        /// Kick-off the process to retrieve an available log for viewing by a consumer.
        /// </summary>
        /// <param name="request">The request.</param>
        void BeginOpenLogFile(Request<FileDownloadOptions> request);

        /// <summary>
        /// Kick-off a RecycleApp event on the client.
        /// </summary>
        /// <param name="request">The request.</param>
        void BeginRecycleClient(Request<Empty> request);
    }
}
