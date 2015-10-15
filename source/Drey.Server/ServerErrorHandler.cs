using Drey.Server.Exceptions;

using Nancy;
using Nancy.Bootstrapper;

using System;

namespace Drey.Server
{
    public class ServerErrorHandler : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError += (ctx, ex) =>
            {
                if (ex.GetBaseException() is TimeoutException)
                {
                    var timeoutExc = ex.GetBaseException() as TimeoutException;
                    return new Response { StatusCode = HttpStatusCode.RequestTimeout, ReasonPhrase = timeoutExc.Message };
                }
                
                if (ex.GetBaseException() is RuntimeHasNotConnectedException)
                {
                    return new Response { StatusCode = HttpStatusCode.ServiceUnavailable, ReasonPhrase = "Runtime is not connected." };
                }

                return null;
            };
        }
    }
}
