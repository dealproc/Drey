using Drey.Nut;
using Nancy.Hosting.Self;
using System;
using System.Diagnostics;
namespace Drey.Configuration
{
    public class Nut : IDisposable
    {
        IDisposable _WebApp;

        public void Configuration(INutConfiguration configurationManager)
        {
            var startupUri = string.Format("http://localhost:{0}/", configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Uri(startupUri));
            host.Start();

            _WebApp = host;
            
            Process.Start(startupUri);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}