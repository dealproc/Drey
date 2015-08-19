using Drey.Nut;
using System;
using System.Threading;

[assembly: Drey.Nut.Cracking(typeof(Samples.AppOne.Nut), true, "Samples - App One", "samples.appone", "Samples - Application One")]
namespace Samples.AppOne
{
    public class Nut : IDisposable
    {
        public void Configuration(INutConfiguration configurationManager)
        {
            Console.WriteLine("Samples - App One Online");
        }

        public bool Shutdown()
        {
            Console.WriteLine("Shutting down after 2 second delay");
            Thread.Sleep(5000);
            return true;
        }

        public void Dispose()
        {
            Console.WriteLine("Samples - App One Torn Down");
        }
    }
}