using Nancy;

using System.Linq;

namespace Samples.Server.Modules
{
    public class HomeModule : NancyModule
    {
        readonly Drey.Server.Infrastructure.IClientRegistry _clientRegistry;

        public HomeModule(Drey.Server.Infrastructure.IClientRegistry clientRegistry) : base("/")
        {
            _clientRegistry = clientRegistry;

            Get["/"] = _ => _clientRegistry.Select(kvp => string.Format("{0}:{1}",kvp.Key, kvp.Value)).Aggregate((s1, s2) => s1 + " | " + s2);
        }
    }
}
