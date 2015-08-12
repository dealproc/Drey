using Nancy.Hosting.Self;
using System;

namespace Samples.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var server = new NancyHost(new Uri("http://localhost:81")))
                {
                    server.Start();
                    Console.WriteLine("Publication Server started.");
                    Console.ReadLine();
                }
            }
            finally { } // squelching any disposal issues.
        }
    }
}