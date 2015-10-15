using Drey.Logging;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Drey.Nut
{
    class ProxyBase : MarshalByRefObject
    {
        static readonly ILog _Log = LogProvider.For<ProxyBase>();
        protected ILog Log { get { return _Log; } }

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

            Console.WriteLine("Attempting to resolve " + asmName);

            Log.Info(() => "Attempting to resolve " + asmName);

            var searchPaths = (new[] 
            { 
                    Path.GetFullPath(_pathToAppPackage), 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
            });

            Log.Debug(() => string.Format("Searching the following locations: {0}", searchPaths.Aggregate((s1, s2) => s1 + ";" + s2)));

            var resolvedDll = searchPaths
                .Select(fullPath => Path.Combine(fullPath, asmName))
                .Select(fullPath => File.Exists(fullPath) ? Assembly.LoadFrom(fullPath) : null)
                .Where(asm => asm != null)
                .FirstOrDefault();

            Log.Trace(() => "Has Dll been resolved? " + (resolvedDll != null ? "Yes" : "No"));

            return resolvedDll;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}