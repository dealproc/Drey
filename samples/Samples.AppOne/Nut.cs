using Drey.Nut;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Samples.AppOne
{
    public class Nut : ShellBase, IDisposable
    {
        public override void Startup(INutConfiguration configurationManager)
        {
            base.Startup(configurationManager);
            Console.WriteLine("Samples - App One Online");
        }

        public override void Shutdown()
        {
            Console.WriteLine("Shutting down after 2 second delay");
            Thread.Sleep(2000);
        }

        public override void Dispose()
        {
            Console.WriteLine("Samples - App One Torn Down");
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
            get { return "Samples - Application #1"; }
        }

        public override bool RequiresConfigurationStorage
        {
            get { return false; }
        }

        public override IEnumerable<DefaultAppSetting> AppSettingDefaults
        {
            get
            {
                yield return new DefaultAppSetting { Key = "Setting.One", Value = string.Empty };
                yield return new DefaultAppSetting { Key = "Setting.Two", Value = string.Empty };
            }
        }

        public override IEnumerable<DefaultConnectionString> ConnectionStringDefaults
        {
            get
            {
                yield return new DefaultConnectionString { Name = "FirstDb", ConnectionString = "", ProviderName = "some.provider" };
            }
        }
    }
}