using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Samples.AppTwo
{
    public class Nut : ShellBase, IDisposable
    {
        public override void Startup(INutConfiguration configurationManager)
        {
            base.Startup(configurationManager);
            Console.WriteLine("Samples - App Two Online");
        }

        public override string Id
        {
            get { return this.GetType().Namespace; }
        }

        public override string NameDomainAs
        {
            get { return this.GetType().Namespace; }
        }

        public override string DisplayAs
        {
            get { return "Samples - Application #2"; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return false; }
        }

        public override void Dispose()
        {
            Console.WriteLine("Samples - App Two Torn Down");
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get { return Enumerable.Empty<DefaultAppSetting>(); }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get { return Enumerable.Empty<DefaultConnectionString>(); }
        }
    }
}