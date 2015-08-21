using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Nut
{
    class ProxyBase : MarshalByRefObject
    {
        readonly string _pathToAppPackage;

        public ProxyBase(string pathToAppPackage)
        {
            _pathToAppPackage = pathToAppPackage;
        }

        public Assembly ResolveAssemblyInDomain(object sender, ResolveEventArgs args)
        {
            var asmName = args.Name + ".dll";
            Console.WriteLine(asmName);

            var foundDll = (new[] 
            { 
                    Path.GetDirectoryName(_pathToAppPackage), 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
            })
                .Select(fullPath => Path.Combine(fullPath, asmName))
                .Select(fullPath => File.Exists(fullPath) ? Assembly.LoadFrom(fullPath) : null)
                .Where(asm => asm != null)
                .FirstOrDefault();

            Console.WriteLine("DLL Found? {0}", foundDll != null);

            return foundDll;
        }
    }
}
