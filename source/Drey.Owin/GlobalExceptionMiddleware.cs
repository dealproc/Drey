using Drey.Owin.Logging;

using Microsoft.Owin;

using System;

namespace Owin
{
    /// <summary>
    /// Captures and stores errors in the owin pipeline that are not handled deeper within the pipeline.
    /// </summary>
    public class GlobalExceptionMiddleware : OwinMiddleware
    {
        static ILog _log = LogProvider.For<GlobalExceptionMiddleware>();

        public GlobalExceptionMiddleware(OwinMiddleware next) : base(next) { }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
