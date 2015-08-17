using Drey.Configuration.Infrastructure.Schema;
using Drey.Nut;

using Nancy.Hosting.Self;

using System;
using System.Diagnostics;

namespace Drey.Configuration
{
    public class Nut : IDisposable
    {
        IPackageEventBus _eventBus;
        IDisposable _webApp;

        public void Configuration(INutConfiguration configurationManager)
        {
            MigrationManager.Migrate(configurationManager);

            _eventBus = configurationManager.EventBus;

            var startupUri = string.Format("http://localhost:{0}/", configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Bootstrapper(configurationManager), new[] { new Uri(startupUri) });
            host.Start();

            _webApp = host;

            Process.Start(startupUri);

            _eventBus.Publish(new PackageEvents.Load { PackageId = "Samples.AppOne", ConfigurationManager = configurationManager });
            _eventBus.Publish(new PackageEvents.Load { PackageId = "Samples.AppTwo", ConfigurationManager = configurationManager });
        }

        public void Dispose()
        {
            _webApp.Dispose();
        }
    }
}