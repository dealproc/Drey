using Drey.Server.Logging;

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
        static readonly ILog _log = LogProvider.For<RuntimeHub>();

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
            _log.Debug("OnConnected called.");
            return _groupMembershipService.Join(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnConnected());
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _log.DebugFormat("OnDisconnected called with stopCalled = {stopCalled}", stopCalled);
            return _groupMembershipService.Leave(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnDisconnected(stopCalled));
        }

        public void EndListLogFiles(DomainModel.Response<IEnumerable<string>> completed)
        {
            _log.DebugFormat("Client returned list of log files.");
            _eventBus.Publish(completed, completed.Token);
        }
        public void EndOpenLogFile(DomainModel.Response<byte[]> completed)
        {
            _log.Debug("Client returned log file.");
            _eventBus.Publish(completed, completed.Token);
        }
        public void EndRecycleClient(DomainModel.Response<DomainModel.Empty> completed)
        {
            _log.Debug("Client is recycling itself.");
            _eventBus.Publish(completed, completed.Token);
        }

        public Task ReportHealth(DomainModel.EnvironmentInfo info)
        {
            _log.Debug("Client reported health.");
            return _clientHealthService.RecordHealthAsync(ConnectedAs, info);
        }
    }
}
