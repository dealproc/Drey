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

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// <remarks>We need to override the default functionality here and send back a `null` so that we can control the lifetime of the ServiceControl.  Default lease time is 5 minutes, which does not work for us.</remarks>
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
