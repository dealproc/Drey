using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Drey.Client.Commands
{
    public class ListReleasesCommand : BaseCommand
    {
        string _url;
        string _packageId;

        public ListReleasesCommand()
        {
            Command = "list-releases";
            Description = "Lists releases for a given package.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v },
                { "p|packageId=", "the package id to query", v => _packageId = v }
            };
        }
        public override bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(_url) && string.IsNullOrWhiteSpace(_packageId));
        }

        public override int Execute()
        {
            var client = new HttpClient { BaseAddress = new Uri(_url) };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var releases = client.GetAsync(".well-known/releases/" + _packageId).Result.Content.ReadAsAsync<IEnumerable<Release>>().Result;

            Console.WriteLine("Found the following packages:");
            Console.WriteLine();

            foreach (var release in releases)
            {
                Console.WriteLine("{0}\t{1}\t{2} b", release.SHA1, release.Filename, release.Filesize);
            }

            return 0;
        }

        class Release
        {
            public string SHA1 { get; set; }
            public string Filename { get; set; }
            public long Filesize { get; set; }
        }
    }
}
