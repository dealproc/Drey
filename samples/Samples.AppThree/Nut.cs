using Drey.Logging;
using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Samples.AppThree
{
    public class Nut : ShellBase, IDisposable
    {
        static ILog _log = LogProvider.For<Nut>();

        public override bool Startup(INutConfiguration configurationManager)
        {
            _log.Info("Samples - App Three Online.");
            _log.Info("App Three should be immediately shutdown because it returns false.");

            base.Startup(configurationManager);

            return false;
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
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }

        protected override void Dispose(bool disposing)
        {
            _log.Info("Samples - App Three Torn Down.");
            base.Dispose(disposing);
        }
    }
}
