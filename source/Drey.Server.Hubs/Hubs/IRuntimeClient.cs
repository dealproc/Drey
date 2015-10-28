using Drey.DomainModel;
namespace Drey.Server.Hubs
{
    public interface IRuntimeClient
    {
        void BeginListLogFiles(Request<Empty> request);
        void BeginOpenLogFile(Request<FileDownloadOptions> request);
        void BeginRecycleClient(Request<Empty> request);
    }
}
