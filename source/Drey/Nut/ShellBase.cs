using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public abstract class ShellBase : MarshalByRefObject, IShell
    {
        public abstract string Id { get; }
        public abstract string NameDomainAs { get; }
        public abstract string DisplayAs { get; }
        public abstract bool RequiresConfigurationStorage { get; }

        public INutConfiguration ConfigurationManager { get; protected set; }

        public virtual void Startup(INutConfiguration configurationManager)
        {
            ConfigurationManager = configurationManager;
        }

        public abstract Task Shutdown();
        public abstract void Dispose();
    }
}