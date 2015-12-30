using Drey.Logging;
using System;

namespace Drey.Utilities
{
    static class AppDomainUtils
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Creates an app domain for loading an app/plugin.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public static AppDomain CreateDomain(string domainName, bool shadowCopyLibraries = false)
        {
            _Log.Trace("Creating a domain.");

            var setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            setup.ShadowCopyFiles = shadowCopyLibraries ? "true" : string.Empty;
            var adEvidence = AppDomain.CurrentDomain.Evidence;

            var domain = AppDomain.CreateDomain(domainName, adEvidence, setup);

            _Log.Trace("Domain created.");
            return domain;
        }
    }
}