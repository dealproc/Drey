﻿using Drey.Configuration.Repositories;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace Drey.Configuration.Services
{
    public class GlobalSettingsService : MarshalByRefObject, IGlobalSettingsService
    {
        readonly IGlobalSettingsRepository _globalSettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSettingsService"/> class.
        /// </summary>
        /// <param name="eventBus">The event bus.</param>
        /// <param name="globalSettingsRepository">The global settings repository.</param>
        public GlobalSettingsService(IGlobalSettingsRepository globalSettingsRepository)
        {
            _globalSettingsRepository = globalSettingsRepository;
        }

        /// <summary>
        /// Stores the initial settings in the Global Settings store.
        /// <remarks>This method should only be used for a new deployment.  Updates to settings should be done via the UpdateServerHostname/UpdateSSLCertificate method(s)</remarks>
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public bool StoreSettings(ViewModels.GlobalSettingsPmo settings)
        {
            UpdateHostDetails(new ViewModels.ServerHostnamePmo()
            {
                NewHostname = settings.ServerHostname,
                NewServerCertificateThumbprint = settings.SSLThumbprint
            });
            UpdateClientCertificate(settings.SSLPfx);

            return true;
        }

        /// <summary>
        /// Gets the server hostname from the Global Settings store.
        /// </summary>
        /// <returns></returns>
        public ViewModels.ServerHostnamePmo GetServerHostname()
        {
            return new ViewModels.ServerHostnamePmo()
            {
                CurrentHostname = _globalSettingsRepository.GetSetting(DreyConstants.ServerHostname),
                CurrentServerCertificateThumbprint = _globalSettingsRepository.GetSetting(DreyConstants.ServerSSLThumbprint)
            };
        }

        /// <summary>
        /// Gets the certificate from the Global Settings store.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 GetCertificate()
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(_globalSettingsRepository.GetSetting(DreyConstants.ClientCertificate));

                if (buffer == null || buffer.Length == 0) { return null; }

                return new X509Certificate2(buffer);
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the server hostname in the Global Settings store.
        /// </summary>
        /// <param name="serverHostName">Name of the server host.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void UpdateHostDetails(ViewModels.ServerHostnamePmo pmo)
        {
            if (string.IsNullOrWhiteSpace(pmo.NewHostname)) { throw new ArgumentNullException(); }
            _globalSettingsRepository.SaveSetting(DreyConstants.ServerHostname, pmo.NewHostname.Trim());
            _globalSettingsRepository.SaveSetting(DreyConstants.ServerSSLThumbprint, (pmo.NewServerCertificateThumbprint ?? string.Empty).Trim());
        }

        /// <summary>
        /// Updates the SSL certificate in the Global Settings store.
        /// </summary>
        /// <param name="cert">The cert.</param>
        /// <exception cref="System.ArgumentNullException">cert</exception>
        public void UpdateClientCertificate(byte[] cert)
        {
            if (cert == null) { throw new ArgumentNullException("cert"); }
            _globalSettingsRepository.SaveSetting(DreyConstants.ClientCertificate, Convert.ToBase64String(cert));
        }

        /// <summary>
        /// Determines whether the global settings are valid or not.
        /// </summary>
        /// <returns></returns>
        public bool HasValidSettings()
        {
            bool hasHostname = !string.IsNullOrWhiteSpace(GetServerHostname().CurrentHostname);
            bool hasCertificate = GetCertificate() != null;
            return hasHostname && hasCertificate;
        }

        /// <summary>
        /// Manufacturers an Http Client with default values.
        /// </summary>
        /// <returns></returns>
        public HttpClient GetHttpClient()
        {
            var url = new Uri(GetServerHostname().CurrentHostname);
            var wrh = new WebRequestHandler();

            var cert = GetCertificate();
            if (cert != null)
            {
                wrh.ClientCertificates.Add(cert);
            }

            var result = new HttpClient(wrh) { BaseAddress = url };

            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return result;
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease" /> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" /> property.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}