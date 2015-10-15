using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IListLogsDirector : IHandle<IEnumerable<string>>
    {
        void Initiate(string clientId, DomainModel.Request<DomainModel.Empty> message);
        Task<IEnumerable<string>> PendingTask { get; }
    }
}
