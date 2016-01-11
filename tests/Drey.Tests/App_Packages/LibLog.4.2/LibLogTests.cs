using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Drey.Tests.App_Packages.LibLog._4._2
{
    public class LibLogTests : IDisposable
    {
        TestTarget _target;
        Logging.ILog _sut;

        public LibLogTests()
        {
            // Configure NLog;
            var config = new NLog.Config.LoggingConfiguration();

            _target = new TestTarget();
            config.AddTarget("target", _target);
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, _target));
            NLog.LogManager.Configuration = config;
            NLog.LogManager.ReconfigExistingLoggers();

            Logging.LogProvider.SetCurrentLogProvider(new Logging.LogProviders.NLogLogProvider());
            _sut = Logging.LogProvider.For<LibLogTests>();
        }



        [Theory]
        [MemberData("Theories")]
        public void Logged_string_should_be_masked(Logging.LogLevel level, string message, string password)
        {
            _sut.Log(Logging.LogLevel.Info, () => message);
            _target.LoggedMessages.ShouldAllBe((msg) => !msg.Contains(password));
        }

        public void Dispose()
        {
            NLog.LogManager.Shutdown();
        }

        public static IEnumerable<object[]> Theories
        {
            get
            {
                var passwd = Guid.NewGuid().ToString();
                return from level in LogLevels
                       from variants in Password_Variants(passwd)
                       select new object[] { level, variants, passwd };
            }
        }

        public static IEnumerable<string> Password_Variants(string plainTextPassword)
        {
            yield return "password=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "pass=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "pwd=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "Password=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "Pass=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "Pwd=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "PASSWORD=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "PASS=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "PWD=shouldbemasked;".Replace("shouldbemasked", plainTextPassword);
            yield return "{ password: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ pass: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ pwd: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ Password: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ Pass: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ Pwd: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ PASSWORD: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ PASS: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ PWD: \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"password\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"pass\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"pwd\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"Password\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"Pass\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"Pwd\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"PASSWORD\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"PASS\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"PWD\": \"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
            yield return "{ \"password\":\"shouldbemasked\" }".Replace("shouldbemasked", plainTextPassword);
        }

        static IEnumerable<Logging.LogLevel> LogLevels
        {
            get { return Enum.GetValues(typeof(Logging.LogLevel)).Cast<Logging.LogLevel>(); }
        }
    }

    [NLog.Targets.Target("TestTarget")]
    sealed class TestTarget : NLog.Targets.Target
    {
        public List<string> LoggedMessages { get; private set; }

        public TestTarget()
        {
            LoggedMessages = new List<string>();
        }

        protected override void Write(NLog.LogEventInfo logEvent)
        {
            LoggedMessages.Add(logEvent.Message);
        }
    }
}
