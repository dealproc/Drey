using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Threading;

namespace Samples.AppOne
{
    [Serializable]
    public class Nut : ShellBase, IDisposable
    {
        static ILog _log = LogProvider.For<Nut>();

        public override void Startup(INutConfiguration configurationManager)
        {
            base.Startup(configurationManager);
            _log.Info("Samples - App One Online");
        }

        public override void Shutdown()
        {
            _log.Info("Shutting down after 2 second delay");
            Thread.Sleep(2000);
        }

        public override string Id
        {
            get { return this.GetType().Namespace; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return false; }
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get
            {
                return new DefaultAppSetting[] {
                    new DefaultAppSetting { Key = "Setting.One", Value = string.Empty },
                    new DefaultAppSetting { Key = "Setting.Two", Value = string.Empty },
                };
            }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get
            {
                yield return new DefaultConnectionString { Name = "FirstDb", ConnectionString = "", ProviderName = "some.provider" };
            }
        }

        protected override void Dispose(bool disposing)
        {
            _log.Info("Samples - App One Torn Down");
            base.Dispose(disposing);
        }
    }
}