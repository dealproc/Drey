using Drey.Logging;
using Drey.Nut;

using NLog;
using NLog.Config;
using NLog.Targets;

using System;

using Topshelf;

namespace Drey.Runtime
{
    class Program
    {
        public static Action<INutConfiguration> LogConfiguration = (INutConfiguration config) =>
        {
            // Step 1. Create configuration object 
            var nlogConfig = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            nlogConfig.AddTarget("console", consoleTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = @"${appdomain:format={1\}} - ${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
            nlogConfig.LoggingRules.Add(rule1);

            // Step 5. Activate the configuration
            LogManager.Configuration = nlogConfig;
            LogManager.ReconfigExistingLoggers();
        };

        static int Main(string[] args)
        {
            return (int)HostFactory.Run(f =>
            {
                f.SetDisplayName("Drey Runtime Environment");
                f.SetServiceName("Runtime");

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
        static ILog _Log = LogProvider.For<HordeServiceWrapper>();

        ControlPanelServiceControl _control;

        public HordeServiceWrapper()
        {
            _control = new ControlPanelServiceControl(ExecutionMode.Development, Program.LogConfiguration);
        }
        public bool Start(HostControl hostControl)
        {
            _Log.Info("Starting Hoarde Service");
            return _control.Start();
        }

        public bool Stop(HostControl hostControl)
        {
            _Log.Info("Stopping Hoarde Service");
            return _control.Stop();
        }
    }
}
