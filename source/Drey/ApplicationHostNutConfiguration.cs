using Drey.Nut;
using System;

namespace Drey
{
    public class ApplicationHostNutConfiguration : MarshalByRefObject, INutConfiguration
    {
        IPackageEventBus _eventBus;
        IApplicationSettings _applicationSettings;
        IGlobalSettings _globalSettings;
        IConnectionStrings _connectionStrings;

        public ApplicationHostNutConfiguration()
        {
            _eventBus = new PackageEventBus();
            
            var appSettings = new AppConfigApplicationSettings();
            _applicationSettings = appSettings;
            _globalSettings = appSettings;

            _connectionStrings = new AppConfigConnectionStrings();
        }

        public IPackageEventBus EventBus
        {
            get { return _eventBus; }
        }

        public IGlobalSettings GlobalSettings
        {
            get { return _globalSettings; }
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