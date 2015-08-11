using Drey.Nut;
using System;

namespace Drey
{
    public class ApplicationHostNutConfiguration : MarshalByRefObject, INutConfiguration
    {
        IPackageEventBus _eventBus;
        IApplicationSettings _applicationSettings;
        IConnectionStrings _connectionStrings;

        public ApplicationHostNutConfiguration()
        {
            _eventBus = new PackageEventBus();
            _applicationSettings = new AppConfigApplicationSettings();
            _connectionStrings = new AppConfigConnectionStrings();
        }

        public IPackageEventBus EventBus
        {
            get { return _eventBus; }
        }

        public IApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
        }

        public IConnectionStrings ConnectionStrings
        {
            get { return _connectionStrings; }
        }

        public string HordeBaseDirectory
        {
            get { return _applicationSettings["horde.directory"]; }
        }
    }
}