using System;

namespace Drey.Server.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception HeadException(this Exception exc)
        {
            if (exc.InnerException == null) { return exc; }

            Exception innerExc = exc;

            do
            {
                innerExc = innerExc.InnerException;
            } while (innerExc.InnerException != null);

            return innerExc;
        }
    }
}
