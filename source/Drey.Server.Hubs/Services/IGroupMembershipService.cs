using Microsoft.AspNet.SignalR;

using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IGroupMembershipService
    {
        /// <summary>
        /// Actions to take when a hub is joined by a client.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="onConnected">The on connected.</param>
        /// <returns></returns>
        Task Join(IHubContext<Hubs.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onConnected);
        
        /// <summary>
        /// Actions to take when a client disconnects from a hub, thereby leaving the context.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="onDisconnected">The on disconnected.</param>
        /// <returns></returns>
        Task Leave(IHubContext<Hubs.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onDisconnected);
    }
}
