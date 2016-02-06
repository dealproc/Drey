using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Manages resolving updated versions of installed packages, downloads them, and signals to the runtime to reload the package at the current release level.
    /// </summary>
    public class ReleasesPollingClient : IPollingClient, IDisposable
    {
        public delegate ReleasesPollingClient Factory(string packageId);

        static readonly ILog _log = LogProvider.For<ReleasesPollingClient>();

        /// <summary>
        /// The delay, in milliseconds, between queries to the known-packages endpoint to see if updates are available.
        /// </summary>
        const int DELAY_TIME_SEC = 15;

        readonly string _packageId;
        readonly Infrastructure.ConfigurationManagement.DbConfigurationSettings.Factory _dbConfigurationSettingsFactory;
        readonly INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.IPackageService _packageService;
        readonly IEventBus _eventBus;

        Task _pollingClientTask;
        CancellationToken _ct;

        bool _disposed = false;

        public string Title { get { return "Releases Polling Client for " + _packageId; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleasesPollingClient" /> class.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="dbConfigurationSettingsFactory">The database configuration settings factory.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="globalSettingsService">The global settings service.</param>
        /// <param name="packageService">The package service.</param>
        /// <param name="eventBus">The event bus.</param>
        public ReleasesPollingClient(string packageId,
            Infrastructure.ConfigurationManagement.DbConfigurationSettings.Factory dbConfigurationSettingsFactory,
            INutConfiguration configurationManager,
            Services.IGlobalSettingsService globalSettingsService,
            Services.IPackageService packageService,
            IEventBus eventBus)
        {
            _packageId = packageId;
            _dbConfigurationSettingsFactory = dbConfigurationSettingsFactory;
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _eventBus = eventBus;
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="ReleasesPollingClient"/> class.
        /// </summary>
        ~ReleasesPollingClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Starts the polling client and query for updated releases.
        /// </summary>
        /// <param name="ct">The ct.</param>
        public void Start(CancellationToken ct)
        {
            _log.Info("Releases Polling Client is starting.");
            _ct = ct;
            _pollingClientTask = new Task(executePollingLoop, ct);
            _pollingClientTask.Start();
        }

        async void executePollingLoop()
        {
            _eventBus.Publish(new ShellRequestArgs
            {
                ActionToTake = ShellAction.Startup,
                PackageId = _packageId,
                Version = string.Empty,
                ConfigurationManager = _dbConfigurationSettingsFactory.Invoke(_packageId)
            });

            while (!_ct.IsCancellationRequested)
            {
                try
                {
                    // poll the releases endpoint
                    var webClient = _globalSettingsService.GetHttpClient();
                    var queryForReleases = await webClient.GetAsync("/.well-known/releases/" + _packageId);
                    var discoveredReleases = await queryForReleases.Content.ReadAsAsync<IEnumerable<DataModel.Release>>();

                    // diff the response with the known releases.
                    var newReleases = _packageService.Diff(_packageId, discoveredReleases);

                    // if the response has new:
                    if (newReleases.Any())
                    {
                        _log.Info("New releases were found.");
                        // determine latest
                        var releaseToDownload = _packageService
                            .GetReleases(_packageId)
                            .Concat(newReleases)
                            .Select(dbRel => new { release = dbRel, Version = new NuGet.SemanticVersion(dbRel.Version) })
                            .OrderByDescending(x => x.Version)
                            .First()
                            .release;

                        await DownloadAndExtractRelease(webClient, releaseToDownload);

                        _packageService.RecordReleases(newReleases);

                        _eventBus.Publish(new ShellRequestArgs
                        {
                            ActionToTake = ShellAction.Restart,
                            PackageId = releaseToDownload.Id,
                            Version = releaseToDownload.Version,
                            ConfigurationManager = _dbConfigurationSettingsFactory.Invoke(_packageId),
                            RemoveOtherVersionsOnRestart = true
                        });
                    }
                    else
                    {
                        _log.Debug("No new releases detected.");
                    }
                }
                catch (Exception ex)
                {
                    _log.ErrorException("Unknown issue occurred", ex);
                }

                _log.DebugFormat("Waiting {0} seconds before checking for new releases.", DELAY_TIME_SEC);

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(DELAY_TIME_SEC), _ct);
                }
                catch (Exception)
                {
                    // squelch.
                }
            }
        }

        private async Task DownloadAndExtractRelease(HttpClient webClient, DataModel.Release releaseToDownload)
        {
            // download latest, based on package id and version (storage in {hordebasedir}\packages
            _log.InfoFormat("Downloading {packageName} - {version}", releaseToDownload.Id, releaseToDownload.Version);
            var fileResult = await webClient.GetAsync("/.well-known/releases/" + releaseToDownload.Id + "/" + releaseToDownload.Version);

            fileResult.EnsureSuccessStatusCode();

            var fileName = fileResult
                .Content
                .Headers
                .First(x => x.Key.Equals("Content-Disposition", StringComparison.InvariantCultureIgnoreCase))
                .Value
                .Single()
                .Replace("attachment; filename=", string.Empty).Replace("\"", string.Empty);

            var destinationFileNameAndPath = Path.Combine(_configurationManager.WorkingDirectory, "packages", fileName);
            var destinationFolder = Path.GetDirectoryName(destinationFileNameAndPath);

            if (!Directory.Exists(destinationFolder))
            {
                _log.TraceFormat("Creating '{destination}'", destinationFolder);
                Directory.CreateDirectory(destinationFolder);
            }

            if (File.Exists(destinationFileNameAndPath))
            {
                _log.Trace("Package exists.  Removing existing package before we store the latest downloaded file.");
                File.Delete(destinationFileNameAndPath);
            }

            _log.InfoFormat("File will be stored at: '{0}'", destinationFileNameAndPath);

            using (var fStream = File.OpenWrite(destinationFileNameAndPath))
            {
                await fileResult.Content.CopyToAsync(fStream);
            }

            var pkg = new NuGet.ZipPackage(destinationFileNameAndPath);
            var zipFileInfo = new FileInfo(fileName);
            var zipFolderName = zipFileInfo.Name;
            zipFolderName = zipFolderName.Substring(0, zipFolderName.Length - zipFileInfo.Extension.Length);
            pkg.ExtractContents(new NuGet.PhysicalFileSystem(_configurationManager.HoardeBaseDirectory), zipFolderName);
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