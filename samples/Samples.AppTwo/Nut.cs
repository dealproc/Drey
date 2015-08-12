using Drey.Nut;
using System;

[assembly: Drey.Nut.Cracking(typeof(Samples.AppTwo.Nut), true, "Samples - App Two")]
namespace Samples.AppTwo
{
    public class Nut : IDisposable
    {
        public void Configuration(INutConfiguration configurationManager)
        {
            Console.WriteLine("Samples - App Two Online");
        }

        public void Dispose()
        {
            Console.WriteLine("Samples - App Two Torn Down");
        }
    }
}