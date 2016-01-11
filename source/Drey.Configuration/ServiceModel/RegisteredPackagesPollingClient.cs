﻿using Drey.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Polls for new packages that are published on the server's package feed.
    /// </summary>
    public class RegisteredPackagesPollingClient : IPollingClient, IDisposable
    {
        static readonly ILog _log = LogProvider.For<RegisteredPackagesPollingClient>();

        const int DELAY_TIME_MS = 60;

        readonly Drey.Nut.INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.IPackageService _packageService;
        readonly Repositories.IPackageSettingRepository _packageSettingRepository;
        readonly Repositories.IConnectionStringRepository _connectionStringRepository;
        readonly PollingClientCollection _pollingClients;
        readonly IEventBus _eventBus;

        Task _pollingClientTask;
        CancellationToken _ct;

        bool _disposed = false;

        public string Title { get { return "Packages Polling Client"; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredPackagesPollingClient"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="globalSettingsService">The global settings service.</param>
        /// <param name="packageService">The package service.</param>
        /// <param name="packageSettingRepository">The package setting repository.</param>
        /// <param name="connectionStringRepository">The connection string repository.</param>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="pollingClients">The polling clients.</param>
        public RegisteredPackagesPollingClient(Drey.Nut.INutConfiguration configurationManager,
            Services.IGlobalSettingsService globalSettingsService,
            Services.IPackageService packageService,
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
        ~RegisteredPackagesPollingClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Starts the Registerd Packages Polling client, and start observing for new packages on the feed.
        /// </summary>
        /// <param name="ct">The ct.</param>
        public void Start(CancellationToken ct)
        {
            _log.Info("Observing for new registered packages to install.");

            _ct = ct;
            _pollingClientTask = new Task(executeLoop, _ct);
            _pollingClientTask.Start();

            var packages = _packageService.GetPackages();
            foreach (var pkg in packages)
            {
                _log.TraceFormat("Creating a release monitor for {packageId}.", pkg.Id);
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
                    _log.Info("Package server could not be contacted.");
                    Pause();
                    continue;
                }
                catch (Exception exc)
                {
                    _log.ErrorException("While discovering new packages", exc);
                    Pause();
                    continue;
                }

                _log.InfoFormat("Waiting {0} seconds before re-checking for new releases.", DELAY_TIME_MS);
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            _disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_pollingClientTask != null)
            {
                _pollingClientTask.Dispose();
                _pollingClientTask = null;
            }

            if (!disposing || _disposed) { return; }
        }
    }
}