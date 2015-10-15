using System;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IOpenLogFileDirector
    {
        void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message);
        Task<byte[]> PendingTask { get; }
    }
}
