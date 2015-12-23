using Drey.Configuration.Infrastructure;
using Drey.Logging;
using Drey.Nut;

using Microsoft.AspNet.SignalR.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    public interface IServicesManager : IHandle<Infrastructure.Events.RecycleApp>
    {
        bool Start();
        bool Stop();
    }
    public class ServicesManager : IServicesManager, IHandle<Infrastructure.Events.RecycleApp>, IDisposable
    {
        ILog _log;

        readonly INutConfiguration _configurationManager;
        readonly IEventBus _eventBus;
        readonly IEnumerable<IRemoteInvocationService> _remoteInvokedServices;
        readonly IEnumerable<IReportPeriodically> _pushServices;
        readonly Services.IGlobalSettingsService _globalSettings;
        readonly Repositories.IPackageRepository _packageRepository;
        readonly Repositories.IConnectionStringRepository _connectionStringsRepository;
        readonly Repositories.IPackageSettingRepository _packageSettingsRepository;
        readonly HoardeManager _hoardeManager;


        IHubConnectionManager _hubConnectionManager;
        IHubProxy _runtimeHubProxy;

        RegisteredPackagesPollingClient _registeredPackagesPoller;
        PollingClientCollection _pollingCollection;

        Func<RegisteredPackagesPollingClient> _packagesPollerFactory;
        Func<PollingClientCollection> _pollingCollectionFactory;


        bool _disposed = false;



        public ServicesManager(INutConfiguration configurationManager, IEventBus eventBus, IEnumerable<IRemoteInvocationService> remoteInvokedServices,
            IEnumerable<IReportPeriodically> pushServices, Services.IGlobalSettingsService globalSettings, Func<RegisteredPackagesPollingClient> packagesPollerFactory,
            Func<PollingClientCollection> pollingCollectionFactory, Repositories.IPackageRepository packageRepository, Repositories.IConnectionStringRepository connectionStringsRepository,
            Repositories.IPackageSettingRepository packageSettingsRepository, HoardeManager hoardeManager)
        {
            _log = LogProvider.For<ServicesManager>();

            _configurationManager = configurationManager;
            _eventBus = eventBus;
            _remoteInvokedServices = remoteInvokedServices;
            _pushServices = pushServices;
            _globalSettings = globalSettings;

            _packagesPollerFactory = packagesPollerFactory;
            _pollingCollectionFactory = pollingCollectionFactory;

            _packageRepository = packageRepository;
            _connectionStringsRepository = connectionStringsRepository;
            _packageSettingsRepository = packageSettingsRepository;

            _hoardeManager = hoardeManager;

            _eventBus.Subscribe(this);
        }

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

        public bool Stop()
        {
            _log.Info("Drey.Runtime is shutting down.");

            var packages = _packageRepository.GetPackages().Where(pkg => pkg.Id != DreyConstants.ConfigurationPackageName);
            
            packages.Apply(p =>
            {
                // Had to refactor from using the event bus due to the event bus not
                // respecting delays from the shutdown process of each package.
                _hoardeManager.Handle(new ShellRequestArgs
                {
                    ActionToTake = ShellAction.Shutdown,
                    PackageId = p.Id,
                    Version = string.Empty,
                    ConfigurationManager = null
                });
            });

            if (_pushServices.Any())
            {
                _pushServices.Apply(x => x.Stop());
            }

            _runtimeHubProxy = null;

            if (_hubConnectionManager != null)
            {
                _hubConnectionManager.Stop();
                _hubConnectionManager.Dispose();
                _hubConnectionManager = null;
            }

            _eventBus.Publish(new ShellRequestArgs
            {
                ActionToTake = ShellAction.Restart,
                PackageId = DreyConstants.ConfigurationPackageName,
                Version = string.Empty,
                ConfigurationManager = null
            });

            return true;
        }

        public void Handle(Infrastructure.Events.RecycleApp message)
        {
            Stop();
            Start();
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

            _log.Trace("Connecting to runtime hub.");

            _hubConnectionManager = HubConnectionManager.GetHubConnectionManager(brokerUrl);
            _hubConnectionManager.UseClientCertificate(_globalSettings.GetCertificate());

            _log.Trace("Creating runtime hub proxy and assigning to services.");
            _runtimeHubProxy = _hubConnectionManager.CreateHubProxy("Runtime");
            _remoteInvokedServices.Apply(x => x.SubscribeToEvents(_runtimeHubProxy));
            _pushServices.Apply(x => x.Start(_hubConnectionManager, _runtimeHubProxy));

            _log.Trace("Establishing connection to runtime hub.");
            _hubConnectionManager.StateChanged += change =>
            {
                switch (change.NewState)
                {
                    case ConnectionState.Connecting:
                        _log.InfoFormat("Attempting to connect to {url}.", brokerUrl);
                        break;
                    case ConnectionState.Connected:
                        _log.InfoFormat("Connection established with {url}.", brokerUrl);
                        break;
                    case ConnectionState.Reconnecting:
                        _log.InfoFormat("Lost connection with {url}.  Attempting to reconnect.", brokerUrl);
                        break;
                    case ConnectionState.Disconnected:
                        _log.InfoFormat("Disconnected from {url}", brokerUrl);
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

            if (_globalSettings.HasValidSettings() && _configurationManager.Mode == ExecutionMode.Production)
            {
                // We need to have valid settings AND we need to be in production mode to start the polling agent(s)
                _registeredPackagesPoller = _packagesPollerFactory.Invoke();
                _pollingCollection = _pollingCollectionFactory.Invoke();
                _pollingCollection.Add(_registeredPackagesPoller);
            }
            else
            {
                // Just discover the packages from the hdd's hoarde directory and start 'em up.
                packages.Apply(p =>
                    _eventBus.Publish(new ShellRequestArgs
                    {
                        ActionToTake = ShellAction.Startup,
                        PackageId = p.Id,
                        Version = string.Empty,
                        ConfigurationManager = new Drey.Configuration.Infrastructure.ConfigurationManagement.DbConfigurationSettings(_configurationManager, _packageSettingsRepository, _connectionStringsRepository, p.Id)
                    }));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            _disposed = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe(this);
            }

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

            if (!disposing || _disposed) { return; }
        }
    }
}
