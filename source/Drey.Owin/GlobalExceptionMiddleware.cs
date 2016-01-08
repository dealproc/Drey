using Drey.Owin.Logging;

using Microsoft.Owin;

using System;

namespace Owin
{
    public class GlobalExceptionMiddleware : OwinMiddleware
    {
        static ILog _log = LogProvider.For<GlobalExceptionMiddleware>();

        public GlobalExceptionMiddleware(OwinMiddleware next) : base(next) { }
        public override async System.Threading.Tasks.Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                _log.FatalException("Global Exception Middleware caught an issue.", ex);
            }
        }
    }
}
