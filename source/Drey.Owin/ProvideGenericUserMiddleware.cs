using Microsoft.Owin;

using System;
using System.Security.Principal;

namespace Owin
{
    public class ProvideGenericUserMiddleware : OwinMiddleware
    {
        public ProvideGenericUserMiddleware(OwinMiddleware next) : base(next) { }
        public override System.Threading.Tasks.Task Invoke(IOwinContext context)
        {
            Console.WriteLine(string.Format("{0}: {1}", context.Request.Method, context.Request.Uri.AbsoluteUri));

            if (context.Request.User == null) { context.Request.User = new GenericPrincipal(new GenericIdentity(""), new string[] { }); }
            return Next.Invoke(context);
        }
    }
}
