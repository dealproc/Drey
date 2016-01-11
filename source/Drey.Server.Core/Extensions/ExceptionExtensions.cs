using System;

namespace Drey.Server.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Pulls the deepest exception out of an exception stack for further processing.
        /// </summary>
        /// <param name="exc">The exc.</param>
        /// <returns></returns>
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
