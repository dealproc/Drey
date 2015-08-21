﻿using Drey.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    public class ShellFactory : MarshalByRefObject
    {
        static readonly ILog _Log = LogProvider.For<ShellFactory>();

        public Tuple<AppDomain, IShell> Create(string assemblyPath, INutConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath)) { return null; }

            _Log.TraceFormat("Loading app from '{0}'", assemblyPath);

            var pathToAssembly = Utilities.PathUtilities.ResolvePath(assemblyPath);
            var discoverStartupType = typeof(DiscoverStartupDllProxy);
            var startupProxyType = typeof(StartupProxy);
            var dreyAssemblyPath = Utilities.PathUtilities.ResolvePath(startupProxyType.Assembly.GetName().CodeBase.Remove(0, 8), false);
            var discoveryDomain = Utilities.AppDomainUtils.CreateDomain(Guid.NewGuid().ToString());
            Tuple<string, string> entryDllAndType;
            DiscoverStartupDllProxy discoverPath = null;

            try
            {
                discoverPath = (DiscoverStartupDllProxy)discoveryDomain.CreateInstanceFromAndUnwrap(discoverStartupType.Assembly.Location, discoverStartupType.FullName, false,
                    BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new[] { pathToAssembly }, null, null);
                discoveryDomain.AssemblyResolve += discoverPath.ResolveAssemblyInDomain;
                entryDllAndType = discoverPath.DiscoverEntryDll(assemblyPath);

                if (entryDllAndType == null)
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
            var domain = Utilities.AppDomainUtils.CreateDomain(Guid.NewGuid().ToString());
            var domainProxy = (StartupProxy)domain.CreateInstanceFromAndUnwrap(startupProxyType.Assembly.Location, startupProxyType.FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new[] { Path.GetDirectoryName(entryDllAndType.Item1) }, null, null);
            domain.AssemblyResolve += domainProxy.ResolveAssemblyInDomain;
            var appShell = (IShell)domainProxy.Build(entryDllAndType.Item1, entryDllAndType.Item2);

            _Log.Info("Running app startup routine.");
            appShell.Startup(config);
            _Log.Info("App startup routine finished.");

            return new Tuple<AppDomain, IShell>(domain, appShell);
        }
    }
}