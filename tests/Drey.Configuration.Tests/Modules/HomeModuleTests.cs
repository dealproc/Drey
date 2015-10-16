using FakeItEasy;

using Nancy;
using Nancy.Testing;

using Shouldly;

using System.Linq;

using Xunit;

namespace Drey.Configuration.Tests.Modules
{
    public class HomeModuleTests
    {
        Fixtures.NancyTestFixture _testFixture;

        public HomeModuleTests()
        {
            _testFixture = new Fixtures.NancyTestFixture();
        }

        [Fact]
        public void Redirects_To_Setup_When_Not_Configured()
        {
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.SeeOther);
            result.Headers.First(x => x.Key == "Location").Value.ShouldBe("/Setup");
        }

        [Fact]
        public void Loads_Index_When_Configured()
        {
            A.CallTo(() => _testFixture.GlobalSettingsService.HasValidSettings()).Returns(true);
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
