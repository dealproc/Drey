using Drey.Server.Infrastructure;
using Drey.Server.Logging;

using Microsoft.AspNet.SignalR;

using System.Security.Claims;
using System.Threading.Tasks;

namespace Samples.Server.Services
{
    public class GroupMembershipService : Drey.Server.Services.IGroupMembershipService
    {
        static readonly ILog _log = LogProvider.For<GroupMembershipService>();

        readonly IClientRegistry<string> _clientRegistry;

        public GroupMembershipService(IClientRegistry<string> clientRegistry)
        {
            _clientRegistry = clientRegistry;
        }

        public Task Join(IHubContext<Drey.DomainModel.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onConnected)
        {
            _log.Info("Joining groups.");

            hubContext.Groups.Add(connectionId, principal.Identity.Name);
            _clientRegistry.Add(connectionId, principal.Identity.Name);

            return onConnected;
        }

        public Task Leave(IHubContext<Drey.DomainModel.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onDisconnected)
        {
            _log.Info("Leaving groups.");

            //hubContext.Groups.Remove(connectionId, principal.Identity.Name);
            _clientRegistry.Remove(connectionId);

            return onDisconnected;
        }
    }
}
