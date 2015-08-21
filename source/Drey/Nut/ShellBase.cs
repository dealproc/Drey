using Drey.Logging;

using System;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public abstract class ShellBase : MarshalByRefObject, IShell
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();
        protected static ILog Log { get { return _Log; } }

        public abstract string Id { get; }
        public abstract string NameDomainAs { get; }
        public abstract string DisplayAs { get; }
        public abstract bool RequiresConfigurationStorage { get; }
        public INutConfiguration ConfigurationManager { get; protected set; }

        public event EventHandler<ShellRequestArgs> OnShellRequest;

        public virtual void Startup(INutConfiguration configurationManager)
        {
            ConfigurationManager = configurationManager;
        }

        public virtual void Shutdown() { }
        public virtual void Dispose() { }

        protected void EmitShellRequest(ShellRequestArgs args)
        {
            var handler = OnShellRequest;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}