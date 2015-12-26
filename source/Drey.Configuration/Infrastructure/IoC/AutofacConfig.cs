using Autofac;

using Drey.Logging;
using Drey.Nut;

using System;

namespace Drey.Configuration.Infrastructure.IoC
{
    static class AutofacConfig
    {
        static ILog _log = LogProvider.GetCurrentClassLogger();
        static IContainer _container;

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

        public static void Configure(IEventBus eventBus, ServiceModel.HoardeManager hoardeManager, INutConfiguration configurationManager)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance<IEventBus>(eventBus);
            builder.RegisterInstance<ServiceModel.HoardeManager>(hoardeManager);
            builder.RegisterInstance<INutConfiguration>(configurationManager);

            builder.RegisterType<ServiceModel.ServicesManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ServiceModel.PollingClientCollection>().AsSelf().SingleInstance();
            builder.RegisterType<ServiceModel.RegisteredPackagesPollingClient>().AsSelf().SingleInstance();

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
