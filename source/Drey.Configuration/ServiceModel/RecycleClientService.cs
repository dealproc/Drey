using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    class RecycleClientService : RemoteInvocationService<DomainModel.Request<DomainModel.Empty>, DomainModel.Empty, DomainModel.Response<DomainModel.Empty>, DomainModel.Empty>
    {
        readonly IEventBus _eventBus;
        public RecycleClientService(IEventBus eventBus) : base("BeginRecycleClient", "EndRecycleClient")
        {
            _eventBus = eventBus;
        }

        protected override Task<DomainModel.Response<DomainModel.Empty>> ProcessAsync(DomainModel.Request<DomainModel.Empty> request)
        {
            _eventBus.Publish(new Infrastructure.Events.RecycleApp());
            return Task.FromResult(DomainModel.Response<DomainModel.Empty>.Success(request.Token, new DomainModel.Empty()));
        }
    }
}
