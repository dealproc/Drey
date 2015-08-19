using Drey.Nut;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Configuration.Services
{
    public class ShellService : IDisposable
    {
        ConcurrentBag<IShell> _shells = new ConcurrentBag<IShell>();

        public ShellService()
        {

        }

        public void Dispose()
        {
            var shutdownTasks = _shells.Select(s => s.Shutdown()).ToArray();
            Task.WaitAll(shutdownTasks);
            shutdownTasks.Apply(t => t.Dispose());
        }
    }
}