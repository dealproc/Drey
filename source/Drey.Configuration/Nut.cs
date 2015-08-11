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
            _eventBus = configurationManager.EventBus;

            var startupUri = string.Format("http://localhost:{0}/", configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Uri(startupUri));
            host.Start();

            _webApp = host;

            Process.Start(startupUri);

            _eventBus.Publish(new PackageEvents.Load { PackageId = "Samples.AppOne" });
            _eventBus.Publish(new PackageEvents.Load { PackageId = "Samples.AppTwo" });
        }

        public void Dispose()
        {
            _webApp.Dispose();
        }
    }
}