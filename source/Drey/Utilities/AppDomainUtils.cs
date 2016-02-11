using Drey.Logging;

using System;
using System.Linq;

namespace Drey.Utilities
{
    static class AppDomainUtils
    {
        static ILog _Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Creates an app domain for loading an app/plugin.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="shadowCopyDirectories">The shadow copy directories.</param>
        /// <returns></returns>
        public static AppDomain CreateDomain(string domainName, string[] shadowCopyDirectories)
        {
            _Log.Trace("Creating a domain.");

            var setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            if (shadowCopyDirectories.Any())
            {
                _Log.Debug("Setting shadow copy directories");
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = string.Join(";", shadowCopyDirectories);
            }

            var adEvidence = AppDomain.CurrentDomain.Evidence;

            var domain = AppDomain.CreateDomain(domainName, adEvidence, setup);

            _Log.Trace("Domain created.");
            return domain;
        }
    }
}