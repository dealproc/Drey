using Drey.Server.Logging;

using System;
using System.Threading.Tasks;

namespace Samples.Server.Services
{
    public class ClientHealthService : Drey.Server.Services.IClientHealthService
    {
        static ILog _log = LogProvider.For<ClientHealthService>();

        public Task RecordHealthAsync(System.Security.Claims.ClaimsPrincipal principal, Drey.DomainModel.EnvironmentInfo healthInfo)
        {
            _log.InfoFormat("Health reported at {time}", DateTime.Now);
            return Task.FromResult<object>(null);
        }

        public void VerifyOnline(System.Security.Claims.ClaimsPrincipal principal)
        {
            
        }
    }
}
