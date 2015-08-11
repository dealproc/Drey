using Mono.Options;
using System;

namespace Drey.Client.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public string Command { get; protected set; }
        public string Description { get; protected set; }
        protected OptionSet Parser { get; set; }

        public virtual bool Matches(string command)
        {
            return command.Equals(Command, StringComparison.InvariantCultureIgnoreCase);
        }
        public void Parse(string[] args)
        {
            try
            {
                Parser.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public abstract bool IsValid();
        public abstract int Execute();
        public void CommandHelp()
        {
            Console.WriteLine("Options:");
            Parser.WriteOptionDescriptions(Console.Out);
        }
    }
}