using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Drey.Nut
{
    class ProxyBase : MarshalByRefObject
    {
        readonly string[] _appPackagePaths;

        //TODO: change to be a params string[] appPackagePaths. document to explain how to utilize.
        public ProxyBase(params string[] appPackagePaths)
        {
            _appPackagePaths = appPackagePaths;
        }

        /// <summary>
        /// Resolves the assembly within the child domain, so we can support assembly unloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>A resolved assembly.</returns>
        public Assembly ResolveAssemblyInDomain(object sender, ResolveEventArgs args)
        {
            var asmName = args.Name.IndexOf(',') > 0 ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;

            asmName = asmName + ".dll";

            var searchPaths = _appPackagePaths.Concat(new[] { Environment.CurrentDirectory }).Distinct();

            var dllFullPath = searchPaths
                .Select(path =>
                {
                    try
                    {
                        // Refactor here to avoid issues with case sensitivity.
                        return Directory
                            .EnumerateFiles(path, "*.dll")
                            .FirstOrDefault(f => f.EndsWith(asmName, StringComparison.OrdinalIgnoreCase));
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                ).Where(s => !string.IsNullOrWhiteSpace(s));

            var resolvedDll = dllFullPath.Where(fullPath => !string.IsNullOrWhiteSpace(fullPath))
                .Select(path => Assembly.LoadFrom(path))
                .Where(asm => asm != null)
                .FirstOrDefault();

            return resolvedDll;
        }
    }
}
