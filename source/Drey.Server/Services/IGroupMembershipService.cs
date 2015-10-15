using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IGroupMembershipService
    {
        Task Join(ClaimsPrincipal principal, string connectionId, Task onConnected);
        Task Leave(ClaimsPrincipal principal, string connectionId, Task onDisconnected);
    }
}
