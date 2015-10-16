using Drey.Configuration.Services;

using FakeItEasy;

using Nancy;
using Nancy.Embedded.Conventions;
using Nancy.ViewEngines;

namespace Drey.Configuration.Tests.Fixtures
{
    public class NancyTestFixture
    {
        public IPackageService PackageService { get; private set; }
        public IGlobalSettingsService GlobalSettingsService { get; private set; }
        public TestNancyBootstrapper Bootstrapper { get; private set; }

        public NancyTestFixture()
        {
            PackageService = A.Fake<IPackageService>();
            GlobalSettingsService = A.Fake<IGlobalSettingsService>();

            Bootstrapper = new TestNancyBootstrapper(this);
        }

    }

    public class TestNancyBootstrapper : DefaultNancyBootstrapper
    {
        NancyTestFixture _testFixture;

        public TestNancyBootstrapper(NancyTestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IPackageService>(_testFixture.PackageService);
            container.Register<IGlobalSettingsService>(_testFixture.GlobalSettingsService);
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions conventions)
        {
            var ThisAssembly = typeof(Drey.Configuration.Nut).Assembly;

            if (!ResourceViewLocationProvider.RootNamespaces.ContainsKey(ThisAssembly))
            {
                ResourceViewLocationProvider.RootNamespaces.Add(ThisAssembly, "Drey.Configuration.Views");
                base.ConfigureConventions(conventions);
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Content", ThisAssembly));
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/fonts", ThisAssembly));
                conventions.StaticContentsConventions.Add(EmbeddedStaticContentConventionBuilder.AddDirectory("/Scripts", ThisAssembly));
            }
        }
    }
}
