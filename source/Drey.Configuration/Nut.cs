using Drey.Configuration.Infrastructure.Schema;
using Drey.Nut;

using Nancy.Hosting.Self;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Drey.Configuration
{
    public class Nut : MarshalByRefObject, IShell, IDisposable
    {
        INutConfiguration _configurationManager;
        IEventBus _eventBus;
        IDisposable _webApp;

        public string Id
        {
            get { return "Drey.Configuration"; }
        }

        public string NameDomainAs
        {
            get { return "Drey.Configuration"; }
        }

        public string DisplayAs
        {
            get { return "Drey Configuration Console"; }
        }

        public bool RequiresConfigurationStorage
        {
            get { return true; }
        }

        public void Startup(INutConfiguration configurationManager)
        {
            MigrationManager.Migrate(configurationManager);

            _configurationManager = configurationManager;
            _eventBus = new EventBus();

            BuildApp();

            var startupUri = string.Format("http://localhost:{0}/", _configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            Process.Start(startupUri);
        }

        private void BuildApp()
        {
            _eventBus.Subscribe(this);

            var startupUri = string.Format("http://localhost:{0}/", _configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Bootstrapper(_configurationManager, _eventBus), new[] { new Uri(startupUri) });
            host.Start();

            _webApp = host;
        }

        public Task Shutdown()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(this);
            _webApp.Dispose();
        }
    }
}