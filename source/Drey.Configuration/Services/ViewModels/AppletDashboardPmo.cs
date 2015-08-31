using System.Collections.Generic;

namespace Drey.Configuration.Services.ViewModels
{
    public class AppletDashboardPmo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the connection strings.
        /// </summary>
        public IEnumerable<AppletConnectionString> ConnectionStrings { get; set; }

        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        public IEnumerable<AppletSetting> AppSettings { get; set; }

        public class AppletConnectionString
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public int Id { get; set; }
            
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// Gets or sets the connection string.
            /// </summary>
            public string ConnectionString { get; set; }

            /// <summary>
            /// Gets or sets the name of the provider.
            /// </summary>
            public string ProviderName { get; set; }
        }

        public class AppletSetting
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            public string Key { get; set; }
            
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }
        }
    }
}