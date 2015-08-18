using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    class ReleasesPollingClient : IPollingClient
    {
        /// <summary>
        /// The delay, in milliseconds, between queries to the known-packages endpoint to see if updates are available.
        /// </summary>
        const int DELAY_TIME_SEC = 5;

        readonly Drey.Nut.INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.PackageService _packageService;
        readonly DataModel.RegisteredPackage _package;
        readonly Drey.IPackageEventBus _packageEventBus;

        Task _pollingClientTask;
        CancellationToken _ct;

        public ReleasesPollingClient(Drey.Nut.INutConfiguration configurationManager, Services.IGlobalSettingsService globalSettingsService, Services.PackageService packageService,
            Drey.IPackageEventBus packageEventBus, DataModel.RegisteredPackage package)
        {
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _packageEventBus = packageEventBus;
            _package = package;
        }

        public void Start(CancellationToken ct)
        {
            _ct = ct;
            _pollingClientTask = new Task(executePollingLoop, ct);
            _pollingClientTask.Start();
        }

        async void executePollingLoop()
        {
            while (!_ct.IsCancellationRequested)
            {
                // poll the releases endpoint
                var webClient = _globalSettingsService.GetHttpClient();
                var discoveredReleases = webClient.GetAsync("/.well-known/releases/" + _package.PackageId).Result.Content.ReadAsAsync<IEnumerable<DataModel.Release>>().Result;

                // diff the response with the known releases.
                var newReleases = _packageService.Diff(_package.PackageId, discoveredReleases);

                // if the response has new:
                if (newReleases.Any())
                {
                    // determine latest
                    var releaseToDownload = _packageService.GetReleases(_package).Concat(newReleases).OrderByDescending(pkg => pkg.Ordinal).SingleOrDefault();

                    // download latest, based on SHA (storage in {hordebasedir}\packages
                    var fileResult = webClient.GetAsync("/.well-known/releases/download/" + releaseToDownload.SHA).Result;

                    fileResult.EnsureSuccessStatusCode();

                    var fileName = fileResult.Headers
                        .First(x => x.Key.Equals("Content-Disposition", StringComparison.InvariantCultureIgnoreCase))
                        .Value
                        .Single()
                        .Replace("filename=", string.Empty).Replace("\"", string.Empty);
                    var destinationFileNameAndPath = Path.Combine(_configurationManager.HordeBaseDirectory, "packages", fileName);

                    using (var fStream = File.OpenWrite(destinationFileNameAndPath))
                    {
                        await fileResult.Content.CopyToAsync(fStream);
                    }

                    // unzip to {hoardbasedir}\{filename} without the extension
                    var zipFileInfo = new FileInfo(fileName);
                    ZipFile.ExtractToDirectory(destinationFileNameAndPath, Path.Combine(_configurationManager.HordeBaseDirectory, zipFileInfo.Name));

                    // signal shutdown of current version
                    _packageEventBus.Publish(new PackageEvents.Shutdown { InstanceId = string.Empty });

                    // signal startup of new version.
                    _packageEventBus.Publish(new PackageEvents.Load { ConfigurationManager = null, PackageId = _package.PackageId });
                }

                await Task.Delay(TimeSpan.FromSeconds(DELAY_TIME_SEC), _ct);
            }
        }
    }
}