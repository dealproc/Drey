using Drey.Logging;
using Drey.Nut;
using System;
using Topshelf;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Drey.DebugRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            HordeServiceControl.ConfigureLogging = (INutConfiguration config) =>
            {
                // Step 1. Create configuration object 
                var nlogConfig = new LoggingConfiguration();

                // Step 2. Create targets and add them to the configuration 
                var consoleTarget = new ColoredConsoleTarget();
                nlogConfig.AddTarget("console", consoleTarget);

                var fileTarget = new FileTarget();
                nlogConfig.AddTarget("file", fileTarget);

                // Step 3. Set target properties 
                consoleTarget.Layout = "${message}";
                fileTarget.FileName = config.LogsDirectory + "log.${machinename}.txt";
                fileTarget.ArchiveFileName = config.LogsDirectory + "archives/log.${machinename}.{#####}.txt";
                fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:maxInnerExceptionLevel=4}";

                // Step 4. Define rules
                var rule1 = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
                nlogConfig.LoggingRules.Add(rule1);

                var rule2 = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
                nlogConfig.LoggingRules.Add(rule2);

                // Step 5. Activate the configuration
                LogManager.Configuration = nlogConfig;
                LogManager.ReconfigExistingLoggers();
                LogManager.Flush();
            };
            HordeServiceControl.ConfigureLogging(new ApplicationHostNutConfiguration());

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
        static ILog _Log = LogProvider.For<HordeServiceWrapper>();

        HordeServiceControl _Control;

        public HordeServiceWrapper()
        {
            _Control = new HordeServiceControl();
        }
        public bool Start(HostControl hostControl)
        {
            _Log.Info("Starting Hoarde Service");
            return _Control.Start();
        }

        public bool Stop(HostControl hostControl)
        {
            _Log.Info("Stopping Hoarde Service");
            return _Control.Stop();
        }
    }
}