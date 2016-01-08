using Drey.Configuration.Repositories;
using Drey.Configuration.Services;

using FakeItEasy;

using Shouldly;

using System;
using System.Collections.Generic;

using Xunit;

namespace Drey.Configuration.Tests.Services
{
    public class GlobalSettingsServiceTests
    {
        IGlobalSettingsRepository _globalSettingsRepository;
        IGlobalSettingsService _SUT;

        public GlobalSettingsServiceTests()
        {
            _globalSettingsRepository = A.Fake<IGlobalSettingsRepository>(opts =>
            {
                opts.Strict();
            });
            _SUT = new GlobalSettingsService(_globalSettingsRepository);
        }

        [Fact]
        public void SettingsCanBeSavedFromPmo()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.Ignored, A<string>.Ignored)).DoesNothing();

            _SUT.StoreSettings(new Configuration.Services.ViewModels.GlobalSettingsPmo { ServerHostname = "http://test.is/", SSLPfx = new byte[10] });
        }

        [Fact]
        public void SSLCertificateCanBeSaved()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ClientCertificate), A<string>.Ignored)).DoesNothing();

            _SUT.UpdateClientCertificate(new byte[10]);

            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ClientCertificate), A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void ServerHostnameCanBeSaved()
        {
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ServerHostname), A<string>.Ignored)).DoesNothing();
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ServerSSLThumbprint), A<string>.Ignored)).DoesNothing();

            _SUT.UpdateHostDetails(new Configuration.Services.ViewModels.ServerHostnamePmo { NewHostname = "http://time.is/utc", NewServerCertificateThumbprint = Guid.NewGuid().ToString() });

            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ServerHostname), A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _globalSettingsRepository.SaveSetting(A<string>.That.IsEqualTo(DreyConstants.ServerSSLThumbprint), A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void CanRetrieveValidSSL()
        {
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).Returns(Convert.ToBase64String(Resources.SampleFiles.testssl));

            var cert = _SUT.GetCertificate();
            cert.ShouldNotBe(null);
            cert.SubjectName.Name.ShouldContain("CN=testssl");
            cert.PrivateKey.ShouldNotBe(null);
        }

        [Fact]
        public void UnsetSSLWillReturnNull()
        {
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo("SSLPfx"))).Returns(null);

            _SUT.GetCertificate().ShouldBe(null);
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
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo(DreyConstants.ServerHostname))).Returns(hostUrl);
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo(DreyConstants.ServerSSLThumbprint))).Returns(string.Empty);
            A.CallTo(() => _globalSettingsRepository.GetSetting(A<string>.That.IsEqualTo(DreyConstants.ClientCertificate))).Returns(sslCert);

            var isValid = _SUT.HasValidSettings();

            isValid.ShouldBe(isConfigured);
        }
    }
}