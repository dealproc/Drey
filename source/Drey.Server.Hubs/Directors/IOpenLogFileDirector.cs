using System;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IOpenLogFileDirector : IHandle<DomainModel.Response<byte[]>>
    {
        void Initiate(string clientId, DomainModel.Request<DomainModel.FileDownloadOptions> message);
        Task<DomainModel.Response<byte[]>> PendingTask { get; }
    }
}
