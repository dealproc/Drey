using Drey.Logging;

using System;
using System.Text;

namespace Drey.Configuration.Infrastructure
{
    /// <summary>
    /// A TextWriter implementation that redirects all values to a liblog log instance, as a Trace event.
    /// </summary>
    class LibLogTraceWriter : System.IO.TextWriter
    {
        static ILog _log = LogProvider.GetCurrentClassLogger();

        StringBuilder sb = new StringBuilder();
        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }

        public override void Write(char value)
        {
            if (value == '\n')
            {
                _log.Trace(sb.ToString());
                sb.Clear();
                return;
            }
            sb.Append(value);
        }
    }
}
