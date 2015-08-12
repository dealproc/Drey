using Mono.Options;
using System;
using System.Net.Http;

namespace Drey.Client.Commands
{
    public class DeleteReleaseCommand : BaseCommand
    {
        string _url;
        string _sha;

        public DeleteReleaseCommand()
        {
            Command = "delete-release";
            Description = "Deletes a release from the repository.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v },
                { "sha=", "the sha of the package to remove", v => _sha = v }
            };
        }
        public override bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(_url) || string.IsNullOrWhiteSpace(_sha));
        }

        public override int Execute()
        {
            try
            {
                var client = new HttpClient { BaseAddress = new Uri(_url) };
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var deleteResult = client.DeleteAsync(".well-known/releases/" + _sha).Result;

                if (deleteResult.IsSuccessStatusCode)
                {
                    Console.WriteLine("Release has been deleted.");
                    return 0;
                }

                // assume bad response.
                Console.WriteLine(deleteResult.ReasonPhrase);
                return -1;
            }
            catch (AggregateException ex)
            {
                Exception exc = ex.InnerException;
                do
                {
                    Console.WriteLine(exc.Message);
                    exc = exc.InnerException;
                } while (exc != null);
                return -99;
            }
        }
    }
}