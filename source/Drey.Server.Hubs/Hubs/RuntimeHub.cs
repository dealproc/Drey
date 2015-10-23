using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Hubs
{
    [HubName("Runtime"), Authorize]
    public class RuntimeHub : Hub<IRuntimeClient>
    {
        readonly IEventBus _eventBus;
        readonly Services.IClientHealthService _clientHealthService;
        readonly Services.IGroupMembershipService _groupMembershipService;
        readonly IHubContext<IRuntimeClient> _runtimeHubContext;

        ClaimsPrincipal ConnectedAs { get { return (ClaimsPrincipal)Context.User; } }

        public RuntimeHub(IEventBus eventBus, Services.IClientHealthService clientHealthService, Services.IGroupMembershipService groupMembershipService,
            IHubContext<IRuntimeClient> runtimeHubContext)
        {
            _eventBus = eventBus;
            _clientHealthService = clientHealthService;
            _groupMembershipService = groupMembershipService;
            _runtimeHubContext = runtimeHubContext;
        }

        public override Task OnConnected()
        {
            return _groupMembershipService.Join(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnConnected());
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return _groupMembershipService.Leave(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnDisconnected(stopCalled));
        }

        public void EndListLogFiles(DomainModel.Response<IEnumerable<string>> completed)
        {
            _eventBus.Publish(completed, completed.Token);
        }
        public void EndOpenLogFile(DomainModel.Response<byte[]> completed)
        {
            _eventBus.Publish(completed, completed.Token);
        }

        public Task ReportHealth(DomainModel.EnvironmentInfo info)
        {
            return _clientHealthService.RecordHealthAsync(ConnectedAs, info);
        }
    }
}
