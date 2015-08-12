using Mono.Options;
using System;
using System.Net.Http;

namespace Drey.Client.Commands
{
    public class DeletePackageCommand : BaseCommand
    {
        string _url;
        string _packageId;

        public DeletePackageCommand()
        {
            Command = "delete-package";
            Description = "Deletes a packages from the repository.";
            Parser = new OptionSet
            {
                { "url=", "the repository's url.", v => _url = v },
                { "p|packageId=", "the package id to query", v => _packageId = v }
            };
        }
        public override bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(_url) || string.IsNullOrWhiteSpace(_packageId));
        }

        public override int Execute()
        {
            try
            {
                var client = new HttpClient { BaseAddress = new Uri(_url) };
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var deleteResult = client.DeleteAsync(".well-known/packages/" + _packageId).Result;

                if (deleteResult.IsSuccessStatusCode)
                {
                    Console.WriteLine("Package has been deleted.");
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