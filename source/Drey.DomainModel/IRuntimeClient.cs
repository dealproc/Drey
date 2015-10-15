namespace Drey.DomainModel
{
    public interface IRuntimeClient
    {
        void BeginListLogFiles(Request<Empty> request);
        void BeginOpenLogFile(Request<FileDownloadOptions> request);
    }
}
