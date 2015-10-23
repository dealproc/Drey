using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IListLogsDirector : IHandle<DomainModel.Response<IEnumerable<string>>>
    {
        void Initiate(string clientId, DomainModel.Request<DomainModel.Empty> message);
        Task<DomainModel.Response<IEnumerable<string>>> PendingTask { get; }
    }
}
