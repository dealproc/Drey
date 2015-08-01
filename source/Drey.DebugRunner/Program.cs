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

                f.Service<HordeServiceControl>();

                f.EnableShutdown();

                f.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                });
            });
        }
    }
}