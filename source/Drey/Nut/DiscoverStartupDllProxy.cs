﻿using Drey.Logging;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Drey.Nut
{
    class DiscoverStartupDllProxy : ProxyBase
    {
        static ILog _log = LogProvider.For<DiscoverStartupDllProxy>();
        public DiscoverStartupDllProxy(params string[] appPackagePaths) : base(appPackagePaths) { }

        /// <summary>
        /// Discovers the entry DLL for the applet.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns></returns>
        public DiscoveredLibraryOptions DiscoverEntryDll(string assemblyPath)
        {
            _log.Info("Attempting to discover entry dll.");
            foreach (var file in Directory.GetFiles(assemblyPath, "*.dll"))
            {
                _log.DebugFormat("--Inspecting: {0}", file);
                var asmToReflect = Assembly.LoadFrom(file);
                Type entryType = asmToReflect.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IShell)));
                if (entryType != null)
                {
                    _log.DebugFormat("--Found entry dll: {0}|{1}", file, entryType.FullName);
                    _log.InfoFormat("Entry Dll discovered: {0}", file);
                    IShell shell = (IShell)asmToReflect.CreateInstance(entryType.FullName);
                    return new DiscoveredLibraryOptions(file, entryType.FullName, shell.Id);
                }
                else
                {
                    _log.Debug("--Dll did not contain a suitable class for startup.");
                }
            }

            _log.Info("No dll was found that contained a suitable entry point for instantiation.");
            return null;
        }
    }
}