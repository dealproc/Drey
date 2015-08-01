using Drey.Nut;
using System.Configuration;

namespace Drey
{
    public class AppConfigConnectionStrings : IConnectionStrings
    {
        public string this[string key]
        {
            get { return ConfigurationManager.ConnectionStrings[key].ConnectionString; }
        }
    }
}