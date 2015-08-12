using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Client.Commands
{
    public class GetReleaseCommand : BaseCommand
    {
        string _url;
        string _sha;
        string _outputFolder;

        public GetReleaseCommand()
        {
            Command = "get-release";
            Description = "Gets a release from the repository, based on its sha1 hash code.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v },
                { "sha=", "the package id to query", v => _sha = v },
                { "of|outputFolder=", "the package to push as the next release.", v => _outputFolder = v },
            };
        }

        public override bool IsValid()
        {
            return !(
                string.IsNullOrWhiteSpace(_url) ||
                string.IsNullOrWhiteSpace(_sha) ||
                string.IsNullOrWhiteSpace(_outputFolder)
            );
        }

        public override int Execute()
        {
            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }

            var client = new HttpClient { BaseAddress = new Uri(_url) };
            var downloadResult = client.GetAsync(".well-known/releases/download/" + _sha).Result;

            if (downloadResult.IsSuccessStatusCode)
            {
                var content = downloadResult.Content;

                if (!(content is StreamContent))
                {
                    Console.WriteLine("Server did not return a stream.");
                    return 1;
                }

                var sContent = content as StreamContent;
                var outputFileName = content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                if (string.IsNullOrWhiteSpace(outputFileName))
                {
                    Console.WriteLine("Server did not return a filename.");
                    return 2;
                }

                using (var fStream = File.OpenWrite(Path.Combine(_outputFolder, outputFileName)))
                {
                    sContent.CopyToAsync(fStream).Wait();
                }

                return 0;
            }

            Console.WriteLine("Response from server did not indicate success:");
            Console.WriteLine(downloadResult.ReasonPhrase);
            return -1;
        }
    }
}
