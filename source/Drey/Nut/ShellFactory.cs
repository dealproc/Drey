using Drey.Logging;
using Drey.Utilities;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace Drey.Nut
{
    public class ShellFactory : MarshalByRefObject
    {
        static readonly ILog _Log = LogProvider.For<ShellFactory>();

        /// <summary>
        /// Creates the specified app.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public Tuple<AppDomain, IShell> Create(string assemblyPath, EventHandler<ShellRequestArgs> shellRequestHandler, Action<INutConfiguration> configureLogging, params string[] additionalSearchPaths)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath)) { return null; }

            _Log.TraceFormat("Loading app from '{0}'", assemblyPath);

            var pathToAssembly = Utilities.PathUtilities.MapPath(assemblyPath);
            var searchPaths = additionalSearchPaths.Select(p => PathUtilities.MapPath(p).NormalizePathSeparator())
                .Concat(new[]
                {
                    Path.GetFullPath(assemblyPath),
                    Environment.CurrentDirectory,
                })
                .ToArray();

            var discoverStartupType = typeof(DiscoverStartupDllProxy);
            var startupProxyType = typeof(StartupProxy);
            var discoveryDomain = Utilities.AppDomainUtils.CreateDomain(Guid.NewGuid().ToString(), searchPaths.ToArray());
            DiscoveredLibraryOptions entryDllOptions;
            DiscoverStartupDllProxy discoverPath = null;

            try
            {
                discoverPath = (DiscoverStartupDllProxy)discoveryDomain.CreateInstanceFromAndUnwrap(discoverStartupType.Assembly.Location, discoverStartupType.FullName, false,
                    BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { searchPaths }, null, null);
                discoveryDomain.AssemblyResolve += discoverPath.ResolveAssemblyInDomain;
                entryDllOptions = discoverPath.DiscoverEntryDll(assemblyPath);

                if (entryDllOptions == null)
                {
                    _Log.Fatal("Installed application does not have a dll with a type that implements Drey.Nut.IShell.  Cannot start.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _Log.FatalException("Could not discover entry dll", ex);
                return null;
            }
            finally
            {
                _Log.Info("Unloading discovery domain.");
                if (discoverPath != null)
                {
                    discoveryDomain.AssemblyResolve -= discoverPath.ResolveAssemblyInDomain;
                }

                AppDomain.Unload(discoveryDomain);
            }

            _Log.Info("Instantiating app.");
            var domain = Utilities.AppDomainUtils.CreateDomain(entryDllOptions.PackageId, searchPaths.ToArray());
            var domainProxy = (StartupProxy)domain.CreateInstanceFromAndUnwrap(startupProxyType.Assembly.Location, startupProxyType.FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { searchPaths }, null, null);
            domain.AssemblyResolve += domainProxy.ResolveAssemblyInDomain;
            var appShell = (IShell)domainProxy.Build(entryDllOptions.File, entryDllOptions.TypeFullName);
            appShell.ConfigureLogging = configureLogging;
            appShell.OnShellRequest += shellRequestHandler;
            appShell.ShellRequestHandler = shellRequestHandler;

            return new Tuple<AppDomain, IShell>(domain, appShell);
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