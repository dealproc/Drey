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
            //domain.AssemblyResolve += (s, e) =>
            //{
            //    var additionalLocations = new[] { @"d:\src\AcademyOne\Drey\horde\drey.configuration-1.0.0.0\" };
            //    var asmName = e.Name + ".dll";
            //    Console.WriteLine(asmName);
            //
            //    var foundDll = additionalLocations.Concat(new[] { Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) })
            //        .Select(fullPath => Path.Combine(fullPath, asmName))
            //        .Select(fullPath => File.Exists(fullPath) ? Assembly.LoadFrom(fullPath) : null)
            //        .Where(asm => asm != null)
            //        .FirstOrDefault();
            //
            //    return foundDll;
            //};

            return domain;
        }
    }
}
