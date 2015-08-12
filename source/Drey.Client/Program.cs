using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drey.Client
{
    class Program
    {
        static Commands.ICommand[] _commands = 
        { 
            (Commands.ICommand)new Commands.DeletePackageCommand(), 
            (Commands.ICommand)new Commands.DeleteReleaseCommand(),
            (Commands.ICommand)new Commands.PushCommand(), 
            (Commands.ICommand)new Commands.PackageCommand(),
            (Commands.ICommand)new Commands.ListPackagesCommand(),
            (Commands.ICommand)new Commands.ListReleasesCommand(),
            (Commands.ICommand)new Commands.GetReleaseCommand()
        };
        static int Main(string[] args)
        {
#if DEBUG
            if (args.Contains("--debug"))
            {
                args = args.Skip(1).ToArray();
                System.Diagnostics.Debugger.Launch();
            }
#endif

            if (args.Length >= 1)
            {
                var command = args.First();
                var commandArgs = args.Skip(1).ToArray();

                var matching = _commands.Where(c => c.Matches(command));

                if (matching.Count() == 1)
                {
                    var cmd = matching.First();
                    cmd.Parse(commandArgs);

                    if (cmd.IsValid())
                    {
                        return cmd.Execute();
                    }

                    cmd.CommandHelp();
                    return -2;
                }
                else
                {
                    DidYouMean(command, matching);
                    return -1;
                }
            }


            ShowHelp();
            return 0;
        }

        private static void DidYouMean(string commandToMatch, IEnumerable<Commands.ICommand> matching)
        {
            Console.WriteLine("\t'{0}' is unknown.  Did you mean:");
            Console.WriteLine();

            foreach (var cmd in matching)
            {
                Console.WriteLine("\t\t'{0}' - {1}", cmd.Command, cmd.Description);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Drey.exe");
            Console.WriteLine();
            foreach (var cmd in _commands)
            {
                Console.WriteLine("\t'{0}' - {1}", cmd.Command, cmd.Description);
            }
        }
    }
}