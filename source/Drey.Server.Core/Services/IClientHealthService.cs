using System.Security.Claims;
using System.Threading.Tasks;

namespace Drey.Server.Services
{
    public interface IClientHealthService
    {
        /// <summary>
        /// Records health statistics reported by a runtime client.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="healthInfo">The health information.</param>
        /// <returns></returns>
        Task RecordHealthAsync(ClaimsPrincipal principal, DomainModel.EnvironmentInfo healthInfo);

        /// <summary>
        /// Allows recording that the reporting runtime is still online.
        /// </summary>
        /// <param name="principal">The principal.</param>
        void VerifyOnline(ClaimsPrincipal principal);
    }
}
