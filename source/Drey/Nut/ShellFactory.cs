using Drey.Logging;
using Drey.Utilities;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.MarshalByRefObject" />
    public class ShellFactory : MarshalByRefObject
    {
        static readonly ILog _Log = LogProvider.For<ShellFactory>();

        /// <summary>
        /// Creates the specified app.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="shellRequestHandler">The shell request handler.</param>
        /// <param name="configureLogging">The configure logging.</param>
        /// <param name="additionalSearchPaths">The additional search paths.</param>
        /// <returns></returns>
        public Tuple<AppDomain, Sponsor<IShell>> Create(string assemblyPath, EventHandler<ShellRequestArgs> shellRequestHandler, Action<INutConfiguration> configureLogging, params string[] additionalSearchPaths)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath)) { return null; }

            _Log.TraceFormat("Loading app from '{0}'", assemblyPath);

            var pathToAssembly = PathUtilities.MapPath(assemblyPath);
            var searchPaths = additionalSearchPaths.Select(p => PathUtilities.MapPath(p).NormalizePathSeparator())
                .Concat(new[]
                {
                    Path.GetFullPath(assemblyPath),
                    Environment.CurrentDirectory,
                })
                .ToArray();

            var discoverStartupType = typeof(DiscoverStartupDllProxy);
            var startupProxyType = typeof(StartupProxy);
            var discoveryDomain = AppDomainUtils.CreateDomain(Guid.NewGuid().ToString(), searchPaths.ToArray());
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
            var domain = AppDomainUtils.CreateDomain(entryDllOptions.PackageId, searchPaths.ToArray());
            var domainProxy = (StartupProxy)domain.CreateInstanceFromAndUnwrap(startupProxyType.Assembly.Location, startupProxyType.FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { searchPaths }, null, null);
            domain.AssemblyResolve += domainProxy.ResolveAssemblyInDomain;
            var appShell = domainProxy.Build(entryDllOptions.File, entryDllOptions.TypeFullName);
            appShell.ConfigureLogging = configureLogging;
            appShell.OnShellRequest += shellRequestHandler;
            appShell.ShellRequestHandler = shellRequestHandler;

            return new Tuple<AppDomain, Sponsor<IShell>>(domain, new Sponsor<IShell>(appShell));
        }
    }
}