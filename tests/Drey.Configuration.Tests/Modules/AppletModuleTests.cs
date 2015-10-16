using FakeItEasy;
using Nancy;
using Nancy.Testing;
using Shouldly;
using Xunit;
using System.Linq;
using System;

namespace Drey.Configuration.Tests.Modules
{
    public class AppletModuleTests
    {
        Fixtures.NancyTestFixture _testFixture;

        public AppletModuleTests()
        {
            _testFixture = new Fixtures.NancyTestFixture();
            A.CallTo(() => _testFixture.GlobalSettingsService.HasValidSettings()).Returns(true);
        }

        [Fact]
        public void CanGetDashboard()
        {
            Console.WriteLine(Environment.CurrentDirectory);

            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/applet/1", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void Can_Get_New_AppSetting_Page()
        {
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/applet/1/appSetting/new", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void Can_Retrieve_Setting_For_Edit()
        {
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/applet/1/appSetting/someSetting/edit", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void Can_Post_Setting_For_Storage()
        {
            var browser = new Browser(_testFixture.Bootstrapper);
            var appSetting = new Drey.Configuration.Services.ViewModels.AppSettingPmo
            {
                Id = 0,
                Key = "SOMEAPPSETTING",
                Value = "APPSETTINGVALUE",
                PackageId = "my.package"
            };

            var result = browser.Post("/applet/1/appSetting", with =>
            {
                with.HttpRequest();
                with.JsonBody(appSetting);
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            A.CallTo(() => _testFixture.PackageService.RecordAppSetting(A<Drey.Configuration.Services.ViewModels.AppSettingPmo>.That.Matches(x => x.PackageId == appSetting.PackageId)))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.StatusCode.ShouldBe(HttpStatusCode.SeeOther);
            result.Headers.First(x => x.Key == "Location").Value.ShouldBe("/applet/" + appSetting.PackageId);
        }

        [Fact]
        public void Can_Get_New_ConnectionString_Page()
        {
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/applet/1/connectionStrings/new", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void Can_Retrieve_ConnectionString_For_Edit()
        {
            var browser = new Browser(_testFixture.Bootstrapper);

            var result = browser.Get("/applet/1/connectionStrings/someSetting/edit", with =>
            {
                with.HttpRequest();
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void Can_Post_ConnectionString_For_Storage()
        {
            var browser = new Browser(_testFixture.Bootstrapper);
            var connectionStringSetting = new Drey.Configuration.Services.ViewModels.ConnectionStringPmo
            {
                Id = 0,
                Name = "MY Connection String",
                ConnectionString = "SOME CONNECTION STRING",
                ProviderName = "SOME.PROVIDER.NAME",
                PackageId = "my.package"
            };

            var result = browser.Post("/applet/1/connectionStrings", with =>
            {
                with.HttpRequest();
                with.JsonBody(connectionStringSetting);
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            A.CallTo(() => _testFixture.PackageService.RecordConnectionString(A<Drey.Configuration.Services.ViewModels.ConnectionStringPmo>.That.Matches(x => x.PackageId == connectionStringSetting.PackageId)))
                .MustHaveHappened(Repeated.Exactly.Once);

            result.StatusCode.ShouldBe(HttpStatusCode.SeeOther);
            result.Headers.First(x => x.Key == "Location").Value.ShouldBe("/applet/" + connectionStringSetting.PackageId);
        }

        [Fact]
        public void Validation_Errors_For_ConnectionString_Do_Not_Redirect()
        {
            var browser = new Browser(_testFixture.Bootstrapper);
            var connectionStringSetting = new Drey.Configuration.Services.ViewModels.ConnectionStringPmo
            {
                Id = 0,
                Name = "",
                ConnectionString = "SOME CONNECTION STRING",
                ProviderName = "SOME.PROVIDER.NAME",
                PackageId = "my.package"
            };

            var result = browser.Post("/applet/1/connectionStrings", with =>
            {
                with.HttpRequest();
                with.JsonBody(connectionStringSetting);
                with.Header("Accept", "application/json; charset=UTF-8");
            });

            A.CallTo(() => _testFixture.PackageService.RecordConnectionString(A<Drey.Configuration.Services.ViewModels.ConnectionStringPmo>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
