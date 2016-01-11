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

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeHub"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="clientHealthService">The client health service.</param>
        /// <param name="groupMembershipService">The group membership service.</param>
        /// <param name="runtimeHubContext">The runtime hub context.</param>
        public RuntimeHub(IEventBus eventBus, Services.IClientHealthService clientHealthService, Services.IGroupMembershipService groupMembershipService,
            IHubContext<IRuntimeClient> runtimeHubContext)
        {
            _eventBus = eventBus;
            _clientHealthService = clientHealthService;
            _groupMembershipService = groupMembershipService;
            _runtimeHubContext = runtimeHubContext;
        }

        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnConnected()
        {
            _log.Debug("OnConnected called.");
            return _groupMembershipService.Join(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnConnected());
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        /// false, if the connection has been lost for longer than the
        /// <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            _log.DebugFormat("OnDisconnected called with stopCalled = {stopCalled}", stopCalled);
            return _groupMembershipService.Leave(_runtimeHubContext, ConnectedAs, Context.ConnectionId, base.OnDisconnected(stopCalled));
        }

        /// <summary>
        /// Handles the response from the client when it completes gathering a list of available log files for viewing.
        /// </summary>
        /// <param name="completed">The completed.</param>
        public void EndListLogFiles(DomainModel.Response<IEnumerable<string>> completed)
        {
            _log.DebugFormat("Client returned list of log files.");
            _eventBus.Publish(completed, completed.Token);
        }
        
        /// <summary>
        /// Handles the response from the client when it returns a log file for a consumer to view.
        /// </summary>
        /// <param name="completed">The completed.</param>
        public void EndOpenLogFile(DomainModel.Response<byte[]> completed)
        {
            _log.Debug("Client returned log file.");
            _eventBus.Publish(completed, completed.Token);
        }
        
        /// <summary>
        /// Ends the recycle client.
        /// </summary>
        /// <param name="completed">The completed.</param>
        public void EndRecycleClient(DomainModel.Response<DomainModel.Empty> completed)
        {
            _log.Debug("Client is recycling itself.");
            _eventBus.Publish(completed, completed.Token);
        }

        /// <summary>
        /// Records reported health details from a client.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns></returns>
        public Task ReportHealth(DomainModel.EnvironmentInfo info)
        {
            _log.Debug("Client reported health.");
            return _clientHealthService.RecordHealthAsync(ConnectedAs, info);
        }
    }
}
