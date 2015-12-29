using Drey.Logging;
using Drey.Nut;

using NLog;
using NLog.Config;
using NLog.Targets;

using System;
using System.IO;

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

            var fileTarget = new FileTarget();
            nlogConfig.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = @"${appdomain:format={0\}-{1\}} - ${logger:shortName} - ${message} ${onexception: ${exception:format=ToString} | ${stacktrace:format=raw} }";
            fileTarget.FileName = Drey.Utilities.PathUtilities.MapPath(Path.Combine(config.LogsDirectory, @"log.${machinename}.${appdomain:format={1\}}.txt"));
            fileTarget.ArchiveFileName = Drey.Utilities.PathUtilities.MapPath(Path.Combine(config.LogsDirectory, @"archives/log.${machinename}.${appdomain:format={1\}}.{#####}.txt"));
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:maxInnerExceptionLevel=4}  ${onexception: ${exception:format=ToString} | ${stacktrace:format=raw} }";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", NLog.LogLevel.Trace, consoleTarget);
            nlogConfig.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
            nlogConfig.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = nlogConfig;
            LogManager.ReconfigExistingLoggers();
        };

        [STAThread]
        public static void Main(string[] args)
        {
            HostFactory.Run(f =>
            {
                f.UseLinuxIfAvailable();

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
            if (!_control.Start())
            {
                _control.Stop();
                return false;
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _Log.Info("Stopping Hoarde Service");
            try
            {
                return _control.Stop();
            }
            finally
            {
                _control.Dispose();
            }
        }
    }
}
