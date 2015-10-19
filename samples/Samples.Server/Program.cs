using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.IO;
using System.Reflection;

namespace Samples.Server {
    class Program {
        static string PACKAGES_DIR = @"c:\packages_test";

        static Autofac.IContainer _container;
        static Drey.Server.Services.IFileService _fileService;

        static void Main(string[] args) {
            BuildContainer();

            if (Directory.Exists(PACKAGES_DIR)) {
                Directory.Delete(PACKAGES_DIR, true);
            }

            var url = "http://localhost:81";

            try {
                using (var webApp = WebApp.Start(url, (app) => {
                    // this is usually in a Startup.cs class, under Configuration(IAppBuilder app) { ... }
                    var hubConfig = new HubConfiguration {
                        EnableDetailedErrors = true,
                        EnableJavaScriptProxies = true,
                        Resolver = new AutofacDependencyResolver(_container)
                    };

                    // hack-ish way to get the IConnectionManager into the Autofac Container.
                    var cb = new ContainerBuilder();
                    cb.RegisterInstance<IConnectionManager>(hubConfig.Resolver.Resolve<IConnectionManager>());
                    cb.Register<IHubContext<Drey.DomainModel.IRuntimeClient>>((ctx) => ctx.Resolve<IConnectionManager>().GetHubContext<Drey.DomainModel.IRuntimeClient>("Runtime"));
                    cb.Update(_container);

                    app.MapSignalR(hubConfig);

                    app.UseNancy(new Nancy.Owin.NancyOptions {
                        Bootstrapper = new SampleServerBootstrapper(_container),
                        EnableClientCertificates = true
                    });
                })) {
                    Console.WriteLine("Publication Server started.");
                    Console.WriteLine("Running on {0}", url);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                }
            } finally { } // squelching any disposal issues.
        }

        static void BuildContainer() {
            _fileService = new Drey.Server.Services.FilesytemFileService(PACKAGES_DIR);

            ContainerBuilder cb = new ContainerBuilder();
            cb.RegisterInstance<Drey.Server.Services.IFileService>(_fileService);
            cb.RegisterType<Drey.Server.EventBus>().AsImplementedInterfaces().SingleInstance();

            cb.RegisterType<Stores.ReleasesStore>().AsImplementedInterfaces();
            cb.RegisterType<Drey.Server.Services.PackageService>().AsImplementedInterfaces();
            cb.RegisterType<Services.ClientHealthService>().AsImplementedInterfaces();
            cb.RegisterType<Services.GroupMembershipService>().AsImplementedInterfaces();

            var serverASM = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Drey.Server.dll"));

            cb.RegisterAssemblyTypes(serverASM)
                .Where(t => t.Name.EndsWith("Director"))
                .AsImplementedInterfaces();

            cb.RegisterHubs(serverASM);

            _container = cb.Build();
        }
    }
}