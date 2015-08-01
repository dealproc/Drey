using Drey.Nut;
using Nancy.Hosting.Self;
using System;
using System.Diagnostics;
namespace Drey.Configuration
{
    public class Nut : IDisposable
    {
        IDisposable _WebApp;

        public void Configuration()
        {
            var startupUri = "http://localhost:8080/";
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