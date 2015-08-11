using Mono.Options;
using System;

namespace Drey.Client.Commands
{
    public class DeleteCommand : BaseCommand
    {
        public DeleteCommand()
        {
            Command = "delete";
            Description = "Deletes a packages from the repository.";
        }
        public override bool IsValid()
        {
            return true;
        }

        public override int Execute()
        {
            Console.WriteLine("Delete Command executing.");
            return 0;
        }
    }
}