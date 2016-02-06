using Drey.Logging;

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

        ReleasesPollingClient.Factory _releasePollingClientFactory;

        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.IPackageService _packageService;
        readonly PollingClientCollection _pollingClients;

        Task _pollingClientTask;
        CancellationToken _ct;

        bool _disposed = false;

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get { return "Packages Polling Client"; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredPackagesPollingClient" /> class.
        /// </summary>
        /// <param name="globalSettingsService">The global settings service.</param>
        /// <param name="packageService">The package service.</param>
        /// <param name="pollingClients">The polling clients.</param>
        /// <param name="releasePollingClientFactory">The release polling client factory.</param>
        public RegisteredPackagesPollingClient(
            Services.IGlobalSettingsService globalSettingsService,
            Services.IPackageService packageService,
            PollingClientCollection pollingClients,
            ReleasesPollingClient.Factory releasePollingClientFactory
            )
        {
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _pollingClients = pollingClients;

            _releasePollingClientFactory = releasePollingClientFactory;
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="RegisteredPackagesPollingClient"/> class.
        /// </summary>
        ~RegisteredPackagesPollingClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Starts the Registered Packages Polling client, and start observing for new packages on the feed.
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
                _pollingClients.Add(_releasePollingClientFactory.Invoke(pkg.Id));
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
                        var clients = newPackages.Select(p => _releasePollingClientFactory.Invoke(p.Id));
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