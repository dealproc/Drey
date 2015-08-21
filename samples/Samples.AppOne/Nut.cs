using Drey.Nut;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.AppOne
{
    public class Nut : ShellBase, IDisposable
    {
        public override void Startup(INutConfiguration configurationManager)
        {
            base.Startup(configurationManager);
            Console.WriteLine("Samples - App One Online");
        }

        public override Task Shutdown()
        {
            Console.WriteLine("Shutting down after 2 second delay");
            Thread.Sleep(5000);
            return Task.FromResult(0);
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
    }
}