using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Client.Commands
{
    public class ListPackagesCommand : BaseCommand
    {
        string _url;

        public ListPackagesCommand()
        {
            Command = "list-packages";
            Description = "Lists packages from the repository.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v }
            };
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(_url);
        }

        public override int Execute()
        {
            var client = new HttpClient { BaseAddress = new Uri(_url) };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var packages = client.GetAsync(".well-known/packages").Result.Content.ReadAsAsync<IEnumerable<Package>>().Result;

            Console.WriteLine("Found the following packages:");
            Console.WriteLine();

            foreach (var package in packages)
            {
                Console.WriteLine("\t" + package.PackageId);
            }

            return 0;
        }


        class Package 
        {
            public string PackageId { get; set; }
        }
    }
}
