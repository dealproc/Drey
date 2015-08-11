using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Client.Commands
{
    public class PushCommand : BaseCommand
    {
        public PushCommand()
        {
            Command = "push";
            Description = "Pushes a package to the repository.";
        }
        public override bool IsValid()
        {
            return true;
        }
        public override int Execute()
        {
            Console.WriteLine("Push Command selected.");
            return 0;
        }
    }
}