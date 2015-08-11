using Drey.Nut;
using Nancy.Hosting.Self;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Configuration
{
    public class Nut : IDisposable
    {
        IDisposable _WebApp;
        CancellationTokenSource _cts = new CancellationTokenSource();
        CancellationToken _ct;
        System.Threading.Tasks.Task _timedClass;

        public void Configuration(INutConfiguration configurationManager)
        {
            var startupUri = string.Format("http://localhost:{0}/", configurationManager.ApplicationSettings["drey.configuration.consoleport"]);
            var host = new NancyHost(new Uri(startupUri));
            host.Start();

            _WebApp = host;

            Process.Start(startupUri);

            _ct = _cts.Token;
            _timedClass = new System.Threading.Tasks.Task(async () =>
            {
                bool isStarting = true;

                while (!_ct.IsCancellationRequested)
                {
                    configurationManager.EventBus.Publish(isStarting ? (object)new PackageEvents.Load() : (object)new PackageEvents.Shutdown());
                    await Task.Delay(10000);
                    isStarting = !isStarting;
                }
            }, _ct);
            _timedClass.Start();
        }

        public void Dispose()
        {
            _WebApp.Dispose();
            _cts.Cancel();
            _timedClass.Wait();
        }
    }
}