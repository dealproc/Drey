using Drey.Nut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    public class DbConfigurationSettings : MarshalByRefObject, Drey.Nut.INutConfiguration
    {
        readonly IApplicationSettings _runtimeApplicationSettings;
        public DbConfigurationSettings(IPackageEventBus eventBus, IApplicationSettings runtimeApplicationSettings)
        {
            EventBus = eventBus;
        }

        public IPackageEventBus EventBus { get; private set; }

        public IApplicationSettings ApplicationSettings
        {
            get { throw new NotImplementedException(); }
        }

        public IConnectionStrings ConnectionStrings
        {
            get { throw new NotImplementedException(); }
        }

        public string HordeBaseDirectory
        {
            get { return _runtimeApplicationSettings["horde.directory"]; }
        }
    }
}
