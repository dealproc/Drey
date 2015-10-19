using Drey.Server.Logging;

using System.Threading.Tasks;

namespace Samples.Server.Services
{
    class GroupMembershipService : Drey.Server.Services.IGroupMembershipService
    {
        static readonly ILog _log = LogProvider.For<GroupMembershipService>();

        public Task Join(System.Security.Claims.ClaimsPrincipal principal, string connectionId, Task onConnected)
        {
            _log.Info("Joining groups.");
            return onConnected;
        }

        public Task Leave(System.Security.Claims.ClaimsPrincipal principal, string connectionId, Task onDisconnected)
        {
            _log.Info("Leaving groups.");
            return onDisconnected;
        }
    }
}
