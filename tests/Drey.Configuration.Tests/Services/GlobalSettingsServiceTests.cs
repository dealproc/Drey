using Drey.Configuration.Repositories;
using Drey.Configuration.Services;
using FakeItEasy;
using System;
using Xunit;
using Shouldly;
using System.Collections.Generic;

namespace Drey.Configuration.Tests.Services
{
    public class GlobalSettingsServiceTests
    {
        IEventBus _eventBus;
        IGlobalSettingsRepository _globalSettingsRepository;
        IGlobalSettingsService _SUT;

        public GlobalSettingsServiceTests()
        {
            _eventBus = A.Dummy<IEventBus>();
            _globalSettingsRepository = A.Fake<IGlobalSettingsRepository>((opts) => opts.Strict());
            _SUT = new GlobalSettingsService(_eventBus, _globalSettingsRepository);
        }

        [Fact]
        public void SettingsCanBeSavedFromPmo()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.Ignored, A<string>.Ignored)).DoesNothing();

            _SUT.StoreSettings(new Configuration.Services.ViewModels.GlobalSettingsPmo { ServerHostname = "http://test.is/", SSLPfx = new byte[10] });

            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void SSLCertificateCanBeSaved()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo("SSLPfx"), A<string>.Ignored)).DoesNothing();

            _SUT.UpdateSSLCertificate(new byte[10]);

            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo("SSLPfx"), A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ServerHostnameCanBeSaved()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo("ServerHostname"), A<string>.Ignored)).DoesNothing();

            _SUT.UpdateServerHostname("http://time.is/utc");

            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo("ServerHostname"), A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void CanRetrieveValidSSL()
        {
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).Returns(Convert.ToBase64String(Resources.SampleFiles.testssl));

            var cert = _SUT.GetCertificate();
            cert.ShouldNotBe(null);
            cert.SubjectName.Name.ShouldContain("CN=testssl");
            cert.PrivateKey.ShouldNotBe(null);

            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).MustHaveHappened();
        }

        [Fact]
        public void UnsetSSLWillReturnNull()
        {
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).Returns(null);

            _SUT.GetCertificate().ShouldBe(null);

            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).MustHaveHappened();
        }

        public static IEnumerable<object[]> InvalidCertificate
        {
            get { return new[] { new object[] { "http://time.is/utc", Convert.ToBase64String(new byte[10]), false } }; }
        }
        public static IEnumerable<object[]> ValidCertificate
        {
            get { return new[] { new object[] { "http://time.is/utc", Convert.ToBase64String(Resources.SampleFiles.testssl), true } }; }
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", null, false)]
        [InlineData("http://time.is/utc", null, false)]
        [InlineData("http://time.is/utc", "", false)]
        [MemberData("InvalidCertificate")]
        [MemberData("ValidCertificate")]
        public void HasValidSettingsTests(string hostUrl, string sslCert, bool isConfigured)
        {
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("ServerHostname"))).Returns(hostUrl);
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).Returns(sslCert);

            var isValid = _SUT.HasValidSettings();
            
            isValid.ShouldBe(isConfigured);
        }
    }
}