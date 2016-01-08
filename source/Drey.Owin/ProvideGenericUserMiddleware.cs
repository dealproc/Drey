using Drey.Owin.Logging;

using Microsoft.Owin;

using System.Security.Principal;

namespace Owin
{
    public class ProvideGenericUserMiddleware : OwinMiddleware
    {
        static ILog _log = LogProvider.For<ProvideGenericUserMiddleware>();

        public ProvideGenericUserMiddleware(OwinMiddleware next) : base(next) { }
        public override System.Threading.Tasks.Task Invoke(IOwinContext context)
        {
            _log.TraceFormat("{method}: {uri}", context.Request.Method, context.Request.Uri.AbsoluteUri);

            if (context.Request.User == null) { context.Request.User = new GenericPrincipal(new GenericIdentity(""), new string[] { }); }
            return Next.Invoke(context);
        }
    }
}
