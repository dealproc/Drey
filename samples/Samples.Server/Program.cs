using Microsoft.Owin.Hosting;

using Owin;

using System;
using System.IO;

namespace Samples.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists(@"c:\packages_test"))
            {
                Directory.Delete(@"c:\packages_test", true);
            }

            var url = "http://localhost:81";

            try
            {
                using (var webApp = WebApp.Start<Startup>(url))
                {
                    Console.WriteLine("Publication Server started.");
                    Console.WriteLine("Running on {0}", url);
                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();
                }
            }
            finally { } // squelching any disposal issues.
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(new Nancy.Owin.NancyOptions
            {
                EnableClientCertificates = true
            });
        }
    }
}