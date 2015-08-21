using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Utilities
{
    static class AppDomainUtils
    {
        public static AppDomain CreateDomain(string domainName)
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var adEvidence = AppDomain.CurrentDomain.Evidence;

            var domain = AppDomain.CreateDomain(domainName, adEvidence, setup);

            return domain;
        }
    }
}
