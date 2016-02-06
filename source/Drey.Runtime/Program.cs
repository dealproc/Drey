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
        /// <summary>
        /// Gets or sets the log verbosity.
        /// <remarks>Used as a temporary storage location for the main app domain.  do not depend on its value in your <code>Action{INutConfiguration} LogConfiguration = (INutConfiguration config) => { ... }</code> implementation.  It will be null for your app domains.</remarks>
        /// </summary>
        public static string LogVerbosity { get; protected set; }

        /// <summary>
        /// Used to establish logging facilities in each package's domain.
        /// <remarks>
        /// <para>
        /// Since we want to control where the log files live, and remove the ability for an end user to dictate this, we are 
        /// configuring the logging provider in code.  The Drey.Configuration package uses the `config.LogsDirectory` for allowing
        /// your portal to access the log files from the machine.
        /// </para>
        /// <para>
        /// Note: Try to keep your log files somewhat small-ish (~150-200k/file).  The file needs to go back to the server and through
        /// a message bus before it gets to your winforms/wpf/webforms/mvc display.
        /// </para>
        /// </remarks>
        /// </summary>
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
            consoleTarget.Layout = @"${appdomain:format={0\} - {1\}} - ${message} ${onexception: ${exception:format=ToString} | ${stacktrace:format=raw} }";

            fileTarget.FileName = Path.Combine(Utilities.PathUtilities.MapPath(config.LogsDirectory), @"log.${appdomain:format={1\}}.txt");
            fileTarget.ArchiveFileName = Path.Combine(Utilities.PathUtilities.MapPath(config.LogsDirectory), @"archives/log.${appdomain:format={1\}}.{#####}.txt");
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:maxInnerExceptionLevel=4}  ${onexception: ${exception:format=ToString} | ${stacktrace:format=raw} }";
            fileTarget.ArchiveAboveSize = 150 * 1024;
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Sequence;

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.FromString(config.LogVerbosity), consoleTarget);
            nlogConfig.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.FromString(config.LogVerbosity), fileTarget);
            nlogConfig.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = nlogConfig;
            LogManager.ReconfigExistingLoggers();

            var log = LogManager.GetLogger("LogConfiguration");
            log.Info("Logs reconfigured.");
            log.Info("Log file name: {0}", fileTarget.FileName);
        };

        [STAThread]
        public static int Main(string[] args)
        {
            LogVerbosity = "Info";

            var instance = HostFactory.New(factory =>
            {
                // parses command line's extra parameters.  format is `-{key}:{value}`
                factory.AddCommandLineDefinition("verbosity", v => LogVerbosity = v);

                // Options to bind to the linux signals for daemon startup/shutdown.
                factory.UseLinuxIfAvailable();

                factory.SetDisplayName("Drey Runtime Environment");
                factory.SetServiceName("Runtime");

                factory.Service(() => new ControlPanelServiceControl(
                    mode: ExecutionMode.Development,
                    configureLogging: LogConfiguration,
                    logVerbosity: LogVerbosity
                ));

                factory.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                });
            });

            return (int)instance.Run();
        }
    }
}
