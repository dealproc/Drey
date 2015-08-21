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
    class ReleasesPollingClient : IPollingClient
    {
        static readonly ILog _Log = LogProvider.For<ReleasesPollingClient>();

        /// <summary>
        /// The delay, in milliseconds, between queries to the known-packages endpoint to see if updates are available.
        /// </summary>
        const int DELAY_TIME_SEC = 5;

        readonly Drey.Nut.INutConfiguration _configurationManager;
        readonly Services.IGlobalSettingsService _globalSettingsService;
        readonly Services.PackageService _packageService;
        readonly IEventBus _eventBus;
        readonly string _packageId;

        Task _pollingClientTask;
        CancellationToken _ct;

        public ReleasesPollingClient(Drey.Nut.INutConfiguration configurationManager, Services.IGlobalSettingsService globalSettingsService, Services.PackageService packageService, IEventBus eventBus, string packageId)
        {
            _configurationManager = configurationManager;
            _globalSettingsService = globalSettingsService;
            _packageService = packageService;
            _eventBus = eventBus;
            _packageId = packageId;
        }

        public void Start(CancellationToken ct)
        {
            _ct = ct;
            _pollingClientTask = new Task(executePollingLoop, ct);
            _pollingClientTask.Start();
        }

        async void executePollingLoop()
        {
            _eventBus.Publish(new ShellRequestArgs { ActionToTake = ShellAction.Startup, PackageId = _packageId, Version = string.Empty });
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
                        // determine latest
                        var releaseToDownload = _packageService
                            .GetReleases(_packageId)
                            .Concat(newReleases)
                            .Select(dbRel => new { release = dbRel, Version = new NuGet.SemanticVersion(dbRel.Version) })
                            .OrderByDescending(x => x.Version)
                            .First()
                            .release;

                        // download latest, based on SHA (storage in {hordebasedir}\packages
                        var fileResult = await webClient.GetAsync("/.well-known/releases/" + releaseToDownload.Id + "/" + releaseToDownload.Version);

                        fileResult.EnsureSuccessStatusCode();

                        var fileName = fileResult
                            .Content
                            .Headers
                            .First(x => x.Key.Equals("Content-Disposition", StringComparison.InvariantCultureIgnoreCase))
                            .Value
                            .Single()
                            .Replace("attachment; filename=", string.Empty).Replace("\"", string.Empty);

                        var destinationFileNameAndPath = Path.Combine(_configurationManager.HordeBaseDirectory, "packages", fileName);
                        var destinationFolder = Path.GetDirectoryName(destinationFileNameAndPath);

                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }

                        if (File.Exists(destinationFileNameAndPath))
                        {
                            File.Delete(destinationFileNameAndPath);
                        }

                        _Log.InfoFormat("File will be stored at: '{0}'", destinationFileNameAndPath);

                        using (var fStream = File.OpenWrite(destinationFileNameAndPath))
                        {
                            await fileResult.Content.CopyToAsync(fStream);
                        }

                        var pkg = new NuGet.ZipPackage(destinationFileNameAndPath);
                        var zipFileInfo = new FileInfo(fileName);
                        var zipFolderName = zipFileInfo.Name;
                        zipFolderName = zipFolderName.Substring(0, zipFolderName.Length - zipFileInfo.Extension.Length);
                        pkg.ExtractContents(new NuGet.PhysicalFileSystem(_configurationManager.HordeBaseDirectory), zipFolderName);

                        _packageService.RecordReleases(newReleases);

                        _eventBus.Publish(new ShellRequestArgs { ActionToTake = Drey.Nut.ShellAction.Restart, PackageId = releaseToDownload.Id, Version = releaseToDownload.Version });
                    }
                }
                catch (Exception ex)
                {
                    _Log.ErrorException("Unknown issue occurred", ex);
                }

                _Log.DebugFormat("Waiting {0} seconds before checking for new releases.", DELAY_TIME_SEC);
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
    }
}