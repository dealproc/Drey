using Microsoft.AspNet.SignalR.Hubs;

using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IGroupMembershipService
    {
        Task Join(IHub hub, ClaimsPrincipal principal, string connectionId, Task onConnected);
        Task Leave(IHub hub, ClaimsPrincipal principal, string connectionId, Task onDisconnected);
    }
}
