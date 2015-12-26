using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Drey.Nut
{
    class ProxyBase : MarshalByRefObject
    {
        readonly string _pathToAppPackage;

        public ProxyBase(string pathToAppPackage)
        {
            _pathToAppPackage = pathToAppPackage;
        }

        /// <summary>
        /// Resolves the assembly within the child domain, so we can support assembly unloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public Assembly ResolveAssemblyInDomain(object sender, ResolveEventArgs args)
        {
            var asmName = args.Name.IndexOf(',') > 0 ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;

            asmName = asmName + ".dll";

#if DEBUG
            Console.WriteLine(Environment.CurrentDirectory);
            Console.WriteLine(_pathToAppPackage);
            Console.WriteLine(asmName);
#endif

            var searchPaths = (new[] 
            { 
                    Path.GetFullPath(_pathToAppPackage), 
                    Path.GetDirectoryName(Environment.CurrentDirectory),
            });

			var dllFullPath = searchPaths
				.Select (path => 
					// Refactor here to avoid issues with case sensitivity.
					Directory
						.EnumerateFiles (path, "*.dll")
						.FirstOrDefault (f => f.EndsWith(asmName, StringComparison.OrdinalIgnoreCase))
				).Where(s => !string.IsNullOrWhiteSpace(s));

			var resolvedDll = dllFullPath.Where (fullPath => !string.IsNullOrWhiteSpace(fullPath))
				.Select(path => Assembly.LoadFrom(path))
                .Where(asm => asm != null)
                .FirstOrDefault();

            return resolvedDll;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
