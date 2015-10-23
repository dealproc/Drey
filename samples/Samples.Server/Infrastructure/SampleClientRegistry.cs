using System.Collections.Concurrent;

namespace Samples.Server.Infrastructure
{
    class SampleClientRegistry : ConcurrentDictionary<string, string>, Drey.Server.Infrastructure.IClientRegistry<string> { }
}
