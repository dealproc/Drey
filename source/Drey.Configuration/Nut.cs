using Drey.Configuration.Infrastructure.Schema;
using Drey.Logging;
using Drey.Nut;

using Nancy.Hosting.Self;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Drey.Configuration
{
    public class Nut : ShellBase, IHandle<ShellRequestArgs>, IDisposable
    {
        INutConfiguration _configurationManager;
        IEventBus _eventBus;
        IDisposable _webApp;

        public static string Version
        {
            get { return typeof(Nut).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; }
        }

        public override string Id
        {
            get { return "Drey.Configuration"; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return true; }
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }

        public override void Startup(INutConfiguration configurationManager)
        {
            MigrationManager.Migrate(configurationManager);

            _configurationManager = configurationManager;
            _eventBus = new EventBus();

            BuildApp();

            var startupUri = string.Format("http://localhost:{0}/", _configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            Process.Start(startupUri);
        }

        public void Handle(ShellRequestArgs message)
        {
            EmitShellRequest(message);
        }

        private void BuildApp()
        {
            _eventBus.Subscribe(this);

            var startupUri = string.Format("http://localhost:{0}/", _configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Bootstrapper(_configurationManager, _eventBus), new[] { new Uri(startupUri) });
            host.Start();

            _webApp = host;
        }

        protected override void Dispose(bool disposing)
        {
            Log.Trace("Disposing Drey Configuration host.");
            
            base.Dispose(disposing);

            if (!disposing) { return; }

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(this);
                _eventBus = null;
            }

            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
            }
        }
    }
}