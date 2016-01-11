using Drey.Logging;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Drey.Tests.App_Packages.LibLog._4._2
{
    public class LibLogTests
    {
        public class MessageTests : Base
        {
            [Theory(Skip = "Invalid data")]
            [MemberData("AllData")]
            public void Messages_with_passwords_are_masked(Logging.LogLevel level, string toMask, string passwd)
            {
                Log.Log(level, () => toMask);
                Target.Output.ShouldAllBe(x => !x.Contains(passwd));
            }
        }

        public class FormatTests : Base
        {
            [Theory(Skip = "Invalid data")]
            [MemberData("AllData")]
            public void Messages_with_passwords_are_masked(Logging.LogLevel level, string toMask, string passwd)
            {


                switch (level)
                {
                    case Logging.LogLevel.Debug: Log.DebugFormat(toMask, passwd); break;
                    case Logging.LogLevel.Error: Log.ErrorFormat(toMask, passwd); break;
                    case Logging.LogLevel.Fatal: Log.FatalFormat(toMask, passwd); break;
                    case Logging.LogLevel.Info: Log.InfoFormat(toMask, passwd); break;
                    case Logging.LogLevel.Trace: Log.TraceFormat(toMask, passwd); break;
                    case Logging.LogLevel.Warn: Log.WarnFormat(toMask, passwd); break;
                }
                Target.Output.ShouldAllBe(x => !x.Contains(passwd));
            }
        }

        public class ExceptionTests : Base
        {
            [Theory(Skip = "Do we need to ensure that logging exceptions masks any passwords within the exception info?")]
            [MemberData("AllData")]
            public void Exceptions_should_be_masked(Logging.LogLevel level, string toMask, string passwd)
            {
                Log.Log(level, () => toMask, new Exception(string.Format("password={0};", passwd)));
                Target.Output.ShouldAllBe(x => !x.Contains(passwd));
            }
        }

        public abstract class Base
        {
            protected ILog Log { get; private set; }
            protected StringListTarget Target { get; private set; }
            public Base()
            {
                var config = new LoggingConfiguration();
                Target = new StringListTarget();
                // layout is used in the runtime.
                Target.Layout = @"${appdomain:format={0\}-{1\}} - ${logger:shortName} - ${message} ${onexception: ${exception:format=ToString} | ${stacktrace:format=raw} }";
                config.AddTarget("StringList", Target);
                config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, Target));
                LogManager.Configuration = config;
                LogManager.ReconfigExistingLoggers();

                Log = LogProvider.For<Base>();
            }

            public static IEnumerable<object[]> AllData
            {
                get
                {
                    return from v in Variations(Guid.NewGuid().ToString())
                           from l in LogLevels
                           select new object[] { l, v.Item1, v.Item2 };
                }
            }
            protected static IEnumerable<Tuple<string, string>> Variations(string plainTextPassword)
            {
                yield return new Tuple<string, string>("password=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("pass=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("pwd=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("Password=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("Pass=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("Pwd=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("PASSWORD=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("PASS=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("PWD=shouldbemasked;".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ password: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ pass: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ pwd: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ Password: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ Pass: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ Pwd: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ PASSWORD: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ PASS: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ PWD: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"password\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"pass\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"pwd\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"Password\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"Pass\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"Pwd\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"PASSWORD\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"PASS\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"PWD\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
                yield return new Tuple<string, string>("{ \"password\":\"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword), plainTextPassword);
            }
            protected static IEnumerable<Logging.LogLevel> LogLevels { get { return Enum.GetValues(typeof(Logging.LogLevel)).Cast<Logging.LogLevel>(); } }
        }

    }
    public class StringListTarget : NLog.Targets.TargetWithLayout
    {
        public List<string> Output { get; private set; }
        public StringListTarget()
        {
            Output = new List<string>();
        }

        protected override void Write(NLog.LogEventInfo logEvent)
        {
            Output.Add(Layout.Render(logEvent));
        }
    }
}
