using Drey.Nut;
using System;

namespace Drey
{
    public class ApplicationHostNutConfiguration : MarshalByRefObject, INutConfiguration
    {
        IApplicationSettings _applicationSettings;
        IConnectionStrings _connectionStrings;

        public ApplicationHostNutConfiguration()
        {
            _applicationSettings = new AppConfigApplicationSettings();
            _connectionStrings = new AppConfigConnectionStrings();
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