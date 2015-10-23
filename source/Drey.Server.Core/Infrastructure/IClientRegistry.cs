using System.Collections.Generic;

namespace Drey.Server.Infrastructure
{
    public interface IClientRegistry<TValue> : IDictionary<string, TValue> { }
}
