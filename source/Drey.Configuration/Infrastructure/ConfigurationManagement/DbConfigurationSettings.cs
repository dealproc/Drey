using Drey.Nut;
using System;

namespace Drey.Configuration.Infrastructure.ConfigurationManagement
{
    public class DbConfigurationSettings : MarshalByRefObject, Drey.Nut.INutConfiguration
    {
        readonly IApplicationSettings _runtimeApplicationSettings;
        public DbConfigurationSettings(IApplicationSettings runtimeApplicationSettings)
        {
            _runtimeApplicationSettings = runtimeApplicationSettings;
        }

        public IGlobalSettings GlobalSettings
        {
            get { throw new NotImplementedException(); }
        }

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