using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IClientHealthService
    {
        Task RecordHealthAsync(ClaimsPrincipal principal, DomainModel.EnvironmentInfo healthInfo);
        void VerifyOnline(ClaimsPrincipal principal);
    }
}
