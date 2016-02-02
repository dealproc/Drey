using Autofac;
using Autofac.Integration.SignalR;

using Drey.Server.Logging;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;

using Owin;

using System;
using System.IdentityModel.Selectors;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Samples.Server
{
    class Program
    {
        static string PACKAGES_DIR = @"c:\packages_test";

        static Autofac.IContainer _container;
        static Drey.Server.Services.IFileService _fileService;

        static void Main(string[] args)
        {
            BuildContainer();

            if (Directory.Exists(PACKAGES_DIR))
            {
                Directory.Delete(PACKAGES_DIR, true);
            }

            var url = "https://+:81";

            try
            {
                using (var webApp = WebApp.Start(url, (app) =>
                {
                    app.UseAutofacMiddleware(_container);

                    app.UseCors(CorsOptions.AllowAll);

                    app.Use<GlobalExceptionMiddleware>();
                    app.Use<BufferContentIntoAMemoryStreamMiddleware>();
                    app.Use<ProvideGenericUserMiddleware>();

                    // auth's the client certificate.
                    app.UseClientCertificateAuthentication(new ClientCertificateAuthenticationOptions
                    {
                        CreateExtendedClaimSet = false,
                        Validator = new customClientCertificateValiator()
                    });

                    // this is usually in a Startup.cs class, under Configuration(IAppBuilder app) { ... }
                    var hubConfig = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        EnableJavaScriptProxies = true,
                        Resolver = new AutofacDependencyResolver(_container)
                    };

                    // hack-ish way to get the IConnectionManager into the Autofac Container.
                    var cb = new ContainerBuilder();
                    cb.RegisterInstance<IConnectionManager>(hubConfig.Resolver.Resolve<IConnectionManager>());
                    cb.Register<IHubContext<Drey.Server.Hubs.IRuntimeClient>>((ctx) => ctx.Resolve<IConnectionManager>().GetHubContext<Drey.Server.Hubs.IRuntimeClient>("Runtime"));
                    cb.Update(_container);

                    app.MapSignalR(hubConfig);

                    app.UseNancy(new Nancy.Owin.NancyOptions
                    {
                        Bootstrapper = new SampleServerBootstrapper(_container),
                        EnableClientCertificates = true
                    });
                }))
                {
                    Console.WriteLine("Publication Server started.");
                    Console.WriteLine("Running on {0}", url);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                }
            }
            finally { } // squelching any disposal issues.
        }

        static void BuildContainer()
        {
            _fileService = new Drey.Server.Services.FilesytemFileService(PACKAGES_DIR);

            ContainerBuilder cb = new ContainerBuilder();
            cb.RegisterInstance<Drey.Server.Services.IFileService>(_fileService);
            cb.RegisterType<Drey.Server.EventBus>().AsImplementedInterfaces().SingleInstance();

            cb.RegisterType<Stores.ReleasesStore>().AsImplementedInterfaces();
            cb.RegisterType<Drey.Server.Services.PackageService>().AsImplementedInterfaces();
            cb.RegisterType<Services.ClientHealthService>().AsImplementedInterfaces();
            cb.RegisterType<Services.GroupMembershipService>().AsImplementedInterfaces();
            cb.RegisterType<Infrastructure.SampleClientRegistry>().AsImplementedInterfaces().SingleInstance();
            cb.RegisterType<Drey.Server.Services.AnonymousNugetApiClaimsValidator>().AsImplementedInterfaces();

            var serverASM = Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Drey.Server.Hubs.dll"));

            cb.RegisterAssemblyTypes(serverASM)
                .Where(t => t.Name.EndsWith("Director"))
                .AsImplementedInterfaces();

            cb.RegisterHubs(serverASM);

            _container = cb.Build();
        }

    }
    // https://github.com/brockallen/BrockAllen.MembershipReboot/blob/master/samples/SingleTenant/SingleTenantWebApp/Areas/UserAccount/Controllers/LoginController.cs

    class customClientCertificateValiator : X509CertificateValidator
    {
        static ILog _log = LogProvider.For<customClientCertificateValiator>();
        /// <summary>
        /// When overridden in a derived class, validates the X.509 certificate.
        /// </summary>
        /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that represents the X.509 certificate to validate.</param>
        /// <exception cref="System.IdentityModel.Tokens.SecurityTokenValidationException">Thrown when the provided certificate is invalid or otherwise cannot be found in the certificate thumbprint store.</exception>
        public override void Validate(X509Certificate2 certificate)
        {
            _log.InfoFormat("Validating {thumbprint}", certificate.Thumbprint);
        }
    }



}