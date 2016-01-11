using System;

namespace Drey.Server.Exceptions
{
    /// <summary>
    /// Thrown when a consumer is making a request to a runtime that has yet to establish a connection with the exchange.
    /// </summary>
    [Serializable]
    public class RuntimeHasNotConnectedException : Exception { }
}
