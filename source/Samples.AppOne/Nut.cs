using Drey.Nut;
using System;

[assembly: Drey.Nut.Cracking(typeof(Samples.AppOne.Nut), true, "Samples - App One")]
namespace Samples.AppOne
{
    public class Nut : IDisposable
    {
        public void Configuration(INutConfiguration configurationManager)
        {
            Console.WriteLine("Samples - App One Online");
        }

        public void Dispose()
        {
            Console.WriteLine("Samples - App One Torn Down");
        }
    }
}