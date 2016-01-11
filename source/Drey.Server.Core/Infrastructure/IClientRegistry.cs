using System.Collections.Generic;

namespace Drey.Server.Infrastructure
{
    /// <summary>
    /// Interface for managing a collection of registered clients.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IClientRegistry<TValue> : IDictionary<string, TValue> { }
}
