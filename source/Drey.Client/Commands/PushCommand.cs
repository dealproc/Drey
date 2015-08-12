using Mono.Options;
using System;
using System.IO;
using System.Net.Http;

namespace Drey.Client.Commands
{
    public class PushCommand : BaseCommand
    {
        string _url;
        string _packageId;
        string _fileName;
        public PushCommand()
        {
            Command = "push";
            Description = "Pushes a release to the repository.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v },
                { "p|packageId=", "the package id to query", v => _packageId = v },
                { "f|file=", "the package to push as the next release.", v => _fileName = v },
            };
        }
        public override bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(_url) || string.IsNullOrWhiteSpace(_packageId) || string.IsNullOrWhiteSpace(_fileName));
        }
        public override int Execute()
        {
            if (!File.Exists(_fileName))
            {
                Console.WriteLine("File does not exist.");
                return -2;
            }

            var postContent = new MultipartFormDataContent();

            var content = new StreamContent(File.OpenRead(_fileName));
            content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("\"{0}\"", (new FileInfo(_fileName).Name))
            };

            postContent.Add(content);

            var client = new HttpClient { BaseAddress = new Uri(_url) };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var queryHttpEndpoint = client.PostAsync(".well-known/releases/" + _packageId, postContent).Result;

            return 0;
        }
    }
}