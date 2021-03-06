﻿using Drey.Configuration.Infrastructure;
using Drey.Logging;
using Drey.Nut;

using Microsoft.AspNet.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServicesManager : IDisposable
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ServicesManager : IServicesManager
    {
        ILog _log;

        readonly INutConfiguration _configurationManager;
        readonly Infrastructure.ConfigurationManagement.DbConfigurationSettings.Factory _dbConfigurationSettingsFactory;
        readonly IEventBus _eventBus;
        readonly IEnumerable<IRemoteInvocationService> _remoteInvokedServices;
        readonly IEnumerable<IReportPeriodically> _pushServices;
        readonly Services.IGlobalSettingsService _globalSettings;
        readonly Repositories.IPackageRepository _packageRepository;
        readonly IHoardeManager _hoardeManager;


        IHubConnectionManager _hubConnectionManager;
        IHubProxy _runtimeHubProxy;

        RegisteredPackagesPollingClient _registeredPackagesPoller;
        PollingClientCollection _pollingCollection;

        Func<RegisteredPackagesPollingClient> _packagesPollerFactory;
        Func<PollingClientCollection> _pollingCollectionFactory;

        bool _disposed = false;

        public ServicesManager(
            INutConfiguration configurationManager,
            Infrastructure.ConfigurationManagement.DbConfigurationSettings.Factory dbConfigurationSettingsFactory,
            IEventBus eventBus,
            IEnumerable<IReportPeriodically> pushServices,
            IEnumerable<IRemoteInvocationService> remoteInvokedServices,
            Services.IGlobalSettingsService globalSettings,
            Func<RegisteredPackagesPollingClient> packagesPollerFactory,
            Func<PollingClientCollection> pollingCollectionFactory,
            Repositories.IPackageRepository packageRepository,
            IHoardeManager hoardeManager)
        {
            _log = LogProvider.For<ServicesManager>();

            _configurationManager = configurationManager;
            _dbConfigurationSettingsFactory = dbConfigurationSettingsFactory;
            _eventBus = eventBus;
            _pushServices = pushServices;
            _remoteInvokedServices = remoteInvokedServices;
            _globalSettings = globalSettings;

            _packagesPollerFactory = packagesPollerFactory;
            _pollingCollectionFactory = pollingCollectionFactory;

            _packageRepository = packageRepository;

            _hoardeManager = hoardeManager;
        }

        /// <summary>
        /// Starts this manager and its services. Establishes connection with the remote server for log file access, etc.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                Connect().Wait();
                InitializePollingClients();
            }
            catch (Exception ex)
            {
                _log.FatalException("Could not connect.", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stops all services managed by this.
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _log.Info("Drey.Runtime is shutting down.");

            if (_pushServices.Any())
            {
                _log.Info("Shutting down push services.");
                Task.WaitAll(_pushServices.Select(x => x.Stop()).ToArray());
                _log.Info("All services should be shut down.");
            }

            _runtimeHubProxy = null;

            if (_hubConnectionManager != null)
            {
                _log.Info("Disposing the connection manager.");
                _hubConnectionManager.Dispose();
                _hubConnectionManager = null;
            }

            return true;
        }

        private Task Connect()
        {
            if (!_globalSettings.HasValidSettings())
            {
                var tsc = new TaskCompletionSource<object>();
                tsc.SetException(new Exception("System has not been setup"));
                return tsc.Task;
            };

            var brokerUrl = _globalSettings.GetServerHostname();

            _log.Debug("Connecting to runtime hub.");

            _hubConnectionManager = HubConnectionManager.GetHubConnectionManager(brokerUrl.CurrentHostname);
            _hubConnectionManager.UseClientCertificate(_globalSettings.GetCertificate());

            _log.Debug("Creating runtime hub proxy and assigning to services.");
            _runtimeHubProxy = _hubConnectionManager.CreateHubProxy("Runtime");

            _log.DebugFormat("# of Remote invoked services registered: {count}", _remoteInvokedServices.Count());
            _remoteInvokedServices.Apply(x => x.SubscribeToEvents(_runtimeHubProxy));

            _log.DebugFormat("# of push services registered: {count}", _pushServices.Count());
            _pushServices.Apply(x => x.Start(_hubConnectionManager, _runtimeHubProxy));

            _log.Debug("Establishing connection to runtime hub.");
            _hubConnectionManager.StateChanged += change =>
            {
                switch (change.NewState)
                {
                    case ConnectionState.Connecting:
                        _log.InfoFormat("Attempting to connect to {url}.", brokerUrl.CurrentHostname);
                        break;
                    case ConnectionState.Connected:
                        _log.InfoFormat("Connection established with {url}.", brokerUrl.CurrentHostname);
                        break;
                    case ConnectionState.Reconnecting:
                        _log.InfoFormat("Lost connection with {url}.  Attempting to reconnect.", brokerUrl.CurrentHostname);
                        break;
                    case ConnectionState.Disconnected:
                        _log.InfoFormat("Disconnected from {url}", brokerUrl.CurrentHostname);
                        break;
                }
            };

            return _hubConnectionManager.Initialize();
        }

        private void InitializePollingClients()
        {
            _log.Info("Initializing Polling Clients...");
            var packages = _packageRepository.GetPackages();

            if (_pollingCollection != null)
            {
                _pollingCollection.Dispose();
                _pollingCollection = null;
            }
            if (_registeredPackagesPoller != null)
            {
                _registeredPackagesPoller.Dispose();
                _registeredPackagesPoller = null;
            }

            _log.InfoFormat("Global Settings are valid: {areSettingsValid}, and we are in {mode} mode.", _globalSettings.HasValidSettings(), _configurationManager.Mode);
            if (_globalSettings.HasValidSettings() && _configurationManager.Mode == ExecutionMode.Production)
            {
                _log.Info("Setting up for discovery of packages from reflector.");

                // We need to have valid settings AND we need to be in production mode to start the polling agent(s)
                _registeredPackagesPoller = _packagesPollerFactory.Invoke();
                _pollingCollection = _pollingCollectionFactory.Invoke();
                _pollingCollection.Add(_registeredPackagesPoller);
            }
            else if (_globalSettings.HasValidSettings())
            {
                _log.Info("Resolving packages from local cache.");
                // Just discover the packages from the hard disk drive's hoarde directory and start 'em up.
                packages.Apply(p =>
                    _eventBus.Publish(new ShellRequestArgs
                    {
                        ActionToTake = ShellAction.Startup,
                        PackageId = p.Id,
                        Version = string.Empty,
                        ConfigurationManager = _dbConfigurationSettingsFactory.Invoke(p.Id)
                    })
                );
            }
            else
            {
                _log.Warn("Did not start polling services, nor could we read from the local store.");
                _log.WarnFormat("Global Settings are valid: {hasValidSettings} | Execution Mode: {executionMode}", _globalSettings.HasValidSettings(), _configurationManager.Mode);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            if (_pollingCollection != null)
            {
                _pollingCollection.Dispose();
                _pollingCollection = null;
            }
            if (_registeredPackagesPoller != null)
            {
                _registeredPackagesPoller.Dispose();
                _registeredPackagesPoller = null;
            }

            if (_runtimeHubProxy != null)
            {
                _runtimeHubProxy = null;
            }

            if (_hubConnectionManager != null)
            {
                _hubConnectionManager.Dispose();
                _hubConnectionManager = null;
            }

            _disposed = true;
        }
    }
}
