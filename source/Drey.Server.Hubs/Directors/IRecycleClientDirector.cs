using System.Threading.Tasks;

namespace Drey.Server.Directors
{
    public interface IRecycleClientDirector : IHandle<DomainModel.Response<DomainModel.Empty>>
    {
        void Initiate(string clientId, DomainModel.Request<DomainModel.Empty> message);
        Task<DomainModel.Response<DomainModel.Empty>> PendingTask { get; }
    }
}
