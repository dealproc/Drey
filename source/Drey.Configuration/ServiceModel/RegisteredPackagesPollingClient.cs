using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    class RegisteredPackagesPollingClient : IPollingClient
    {
        const int DELAY_TIME_MS = 60;

        readonly INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.PackageService _packageService;
        readonly PollingClientCollection _pollingClients;
        
        Task _pollingClientTask;
        CancellationToken _ct;

        public RegisteredPackagesPollingClient(INutConfiguration configurationManager, Services.IGlobalSettingsService globalSettingsService, Services.PackageService packageService, PollingClientCollection pollingClients)
        {
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _pollingClients = pollingClients;
        }

        public void Start(CancellationToken ct)
        {
            _ct = ct;
            _pollingClientTask = new Task(executeLoop, _ct);
            _pollingClientTask.Start();

            var packages = _packageService.GetPackages();
            foreach (var pkg in packages)
            {
                _pollingClients.Add(new ReleasesPollingClient(_configurationManager, _globalSettingsService, _packageService, pkg.Id));
            }
        }

        async void executeLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                var webClient = _globalSettingsService.GetHttpClient();

                var packages = webClient.GetAsync("/.well-known/packages").Result.Content.ReadAsAsync<IEnumerable<DataModel.Package>>().Result;

                var knownPackages = _packageService.GetPackages().Select(pkg=>pkg.Id).ToArray();
                var newPackages = packages.Where(p => !knownPackages.Contains(p.Id)).ToList();


                if (newPackages.Any())
                {
                    var clients = newPackages.Select(p => new ReleasesPollingClient(_configurationManager, _globalSettingsService, _packageService, p.Id));
                    foreach (var client in clients)
                    {
                        _pollingClients.Add(client);
                    }
                }

                Console.WriteLine("Waiting {0} seconds before re-checking for new releases.", DELAY_TIME_MS);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(DELAY_TIME_MS), _ct);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}