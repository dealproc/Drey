using Autofac;

using Drey.Logging;
using Drey.Nut;

using System;

namespace Drey.Configuration.Infrastructure.IoC
{
    /// <summary>
    /// Autofac configuration.
    /// <remarks>Configuration routine for autofac, as well as holds the static instance of the configured container.</remarks>
    /// </summary>
    static class AutofacConfig
    {
        static ILog _log = LogProvider.GetCurrentClassLogger();
        static IContainer _container;

        /// <summary>
        /// The configured autofac container.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Autofac Container has not been configured.</exception>
        public static IContainer Container
        {
            get
            {
                if (_container == null) 
                {
                    throw new InvalidOperationException("Autofac Container has not been configured.");
                }
                return _container;
            }
        }

        /// <summary>
        /// Configuration routine of the autofac container.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="hoardeManager">The hoarde manager.</param>
        /// <param name="configurationManager">The host's configuration manager.</param>
        public static void Configure(IEventBus eventBus, ServiceModel.IHoardeManager hoardeManager, INutConfiguration configurationManager)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(eventBus);
            builder.RegisterInstance(hoardeManager);
            builder.RegisterInstance(configurationManager);

            builder.RegisterType<ServiceModel.ServicesManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServiceModel.PollingClientCollection>().AsSelf().SingleInstance();
            builder.RegisterType<ServiceModel.RegisteredPackagesPollingClient>().AsSelf().SingleInstance();
            builder.RegisterType<ServiceModel.ReleasesPollingClient>();
            builder.RegisterType<ConfigurationManagement.DbConfigurationSettings>();

            // we will apply most of the configuration in one or more assembly modules.
            builder.RegisterAssemblyModules(typeof(AutofacConfig).Assembly);

            switch (configurationManager.Mode)
            {
                case ExecutionMode.Development:
                    builder.RegisterType<Repositories.OnDisk.OnDiskPackageRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    break;
                case ExecutionMode.Production:
                    builder.RegisterType<Repositories.SQLiteRepositories.PackageRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    break;
                default:
                    builder.RegisterType<Repositories.SQLiteRepositories.PackageRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
                    _log.WarnFormat("Unknown execution mode '{mode}'.  Registered Sqlite Repository.", configurationManager.Mode);
                    break;
            }

            _container = builder.Build();
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
        public static void DisposeContainer()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }
    }
}
