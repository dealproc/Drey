using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Services.ViewModels
{
    public class AppletDashboardPmo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }

        public IEnumerable<AppletConnectionString> ConnectionStrings { get; set; }
        public IEnumerable<AppletSetting> AppSettings { get; set; }

        public class AppletConnectionString
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }

        public class AppletSetting
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
