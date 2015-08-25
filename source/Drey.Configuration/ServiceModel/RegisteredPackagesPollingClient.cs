using Drey.Logging;

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
        static readonly ILog _Log = LogProvider.For<RegisteredPackagesPollingClient>();

        const int DELAY_TIME_MS = 60;

        readonly Drey.Nut.INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.PackageService _packageService;
        readonly Repositories.IPackageSettingRepository _packageSettingRepository;
        readonly Repositories.IConnectionStringRepository _connectionStringRepository;
        readonly PollingClientCollection _pollingClients;
        readonly IEventBus _eventBus;

        Task _pollingClientTask;
        CancellationToken _ct;

        public RegisteredPackagesPollingClient(Drey.Nut.INutConfiguration configurationManager,
            Services.IGlobalSettingsService globalSettingsService,
            Services.PackageService packageService,
            Repositories.IPackageSettingRepository packageSettingRepository,
            Repositories.IConnectionStringRepository connectionStringRepository,
            IEventBus eventBus,
            PollingClientCollection pollingClients)
        {
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _packageSettingRepository = packageSettingRepository;
            _connectionStringRepository = connectionStringRepository;
            _pollingClients = pollingClients;
            _eventBus = eventBus;
        }

        public void Start(CancellationToken ct)
        {
            _ct = ct;
            _pollingClientTask = new Task(executeLoop, _ct);
            _pollingClientTask.Start();

            var packages = _packageService.GetPackages();
            foreach (var pkg in packages)
            {
                _pollingClients.Add(CreateReleasesMonitor(pkg.Id));
            }
        }

        async void executeLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                List<DataModel.Package> newPackages;

                try
                {
                    var webClient = _globalSettingsService.GetHttpClient();

                    var webClientResponse = await webClient.GetAsync("/.well-known/packages");
                    webClientResponse.EnsureSuccessStatusCode();

                    var packages = await webClientResponse.Content.ReadAsAsync<IEnumerable<DataModel.Package>>();

                    var knownPackages = _packageService.GetPackages().Select(pkg => pkg.Id).ToArray();
                    newPackages = packages.Where(p => !knownPackages.Contains(p.Id)).ToList();


                    if (newPackages.Any())
                    {
                        var clients = newPackages.Select(p => CreateReleasesMonitor(p.Id));
                        foreach (var client in clients)
                        {
                            _pollingClients.Add(client);
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    _Log.Info("Package server could not be contacted.");
                    Pause();
                    continue;
                }
                catch (Exception exc)
                {
                    _Log.ErrorException("While discovering new packages", exc);
                    Pause();
                    continue;
                }

                _Log.InfoFormat("Waiting {0} seconds before re-checking for new releases.", DELAY_TIME_MS);
                Pause();
            }
        }

        private ReleasesPollingClient CreateReleasesMonitor(string packageId)
        {
            return new ServiceModel.ReleasesPollingClient(_configurationManager, _globalSettingsService, _packageService, _packageSettingRepository, _connectionStringRepository, _eventBus, packageId);
        }

        void Pause()
        {
            try
            {
                Task.Delay(TimeSpan.FromSeconds(DELAY_TIME_MS), _ct).Wait();
            }
            catch (Exception)
            {
                // squashing.
            }
        }
    }
}