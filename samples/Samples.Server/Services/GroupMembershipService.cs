using Drey.Server.Infrastructure;
using Drey.Server.Logging;

using Microsoft.AspNet.SignalR.Hubs;

using System.Threading.Tasks;

namespace Samples.Server.Services
{
    public class GroupMembershipService : Drey.Server.Services.IGroupMembershipService
    {
        static readonly ILog _log = LogProvider.For<GroupMembershipService>();

        readonly IClientRegistry _clientRegistry;

        public GroupMembershipService(IClientRegistry clientRegistry)
        {
            _clientRegistry = clientRegistry;
        }

        public Task Join(IHub hub, System.Security.Claims.ClaimsPrincipal principal, string connectionId, Task onConnected)
        {
            _log.Info("Joining groups.");

            hub.Groups.Add(connectionId, principal.Identity.Name);
            _clientRegistry.Add(connectionId, principal.Identity.Name);

            return onConnected;
        }

        public Task Leave(IHub hub, System.Security.Claims.ClaimsPrincipal principal, string connectionId, Task onDisconnected)
        {
            _log.Info("Leaving groups.");

            //hub.Groups.Remove(connectionId, principal.Identity.Name);
            _clientRegistry.Remove(connectionId);

            return onDisconnected;
        }
    }
}
