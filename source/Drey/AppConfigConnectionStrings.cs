using Drey.Nut;
using System;
using System.Configuration;

namespace Drey
{
    public class AppConfigConnectionStrings : MarshalByRefObject, IConnectionStrings
    {
        public string this[string key]
        {
            get { return ConfigurationManager.ConnectionStrings[key].ConnectionString; }
        }

        public void Register(System.Collections.Generic.IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}