using Microsoft.AspNet.SignalR;

using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IGroupMembershipService
    {
        Task Join(IHubContext<Hubs.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onConnected);
        Task Leave(IHubContext<Hubs.IRuntimeClient> hubContext, ClaimsPrincipal principal, string connectionId, Task onDisconnected);
    }
}
