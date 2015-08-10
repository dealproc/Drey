using System;
using Topshelf;

namespace Drey.DebugRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(f =>
            {
                f.SetInstanceName("DebugRunner");
                f.SetDisplayName("Horde Debug Runner");
                f.SetServiceName("DebugRunner");

                f.Service<HordeServiceWrapper>();

                f.EnableShutdown();

                f.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                });
            });
        }
    }

    class HordeServiceWrapper : ServiceControl
    {
        HordeServiceControl _Control;

        public HordeServiceWrapper()
        {
            _Control = new HordeServiceControl();
        }
        public bool Start(HostControl hostControl)
        {
            return _Control.Start();
        }

        public bool Stop(HostControl hostControl)
        {
            return _Control.Stop();
        }
    }
}