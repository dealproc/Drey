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
        readonly Drey.IPackageEventBus _packageEventBus;

        Task _pollingClientTask;
        CancellationToken _ct;

        public RegisteredPackagesPollingClient(INutConfiguration configurationManager, Services.IGlobalSettingsService globalSettingsService, Services.PackageService packageService,
            Drey.IPackageEventBus packageEventBus, PollingClientCollection pollingClients)
        {
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _packageEventBus = packageEventBus;
            _pollingClients = pollingClients;
        }

        public void Start(CancellationToken ct)
        {
            _ct = ct;
            _pollingClientTask = new Task(executeLoop, _ct);
            _pollingClientTask.Start();

            var packages = _packageService.GetRegisteredPackages();
            foreach (var pkg in packages)
            {
                _pollingClients.Add(new ReleasesPollingClient(_configurationManager, _globalSettingsService, _packageService, _packageEventBus, pkg));
            }
        }

        async void executeLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                var webClient = _globalSettingsService.GetHttpClient();

                var packages = webClient.GetAsync("/.well-known/packages").Result.Content.ReadAsAsync<IEnumerable<DataModel.RegisteredPackage>>().Result;

                var newPackages = _packageService.RegisterNewPackages(packages);

                if (newPackages.Any())
                {
                    var clients = newPackages.Select(p => new ReleasesPollingClient(_configurationManager, _globalSettingsService, _packageService, _packageEventBus, p));
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