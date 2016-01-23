using System.Security.Claims;

namespace Drey.Server.Services
{
    public interface INugetApiClaimsValidator
    {
        bool Validate(Claim[] claims);
    }

    public class AnonymousNugetApiClaimsValidator : INugetApiClaimsValidator
    {
        public bool Validate(Claim[] claims)
        {
            return true;
        }
    }
}
