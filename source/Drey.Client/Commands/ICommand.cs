namespace Drey.Client.Commands
{
    public interface ICommand
    {
        string Command { get; }
        string Description { get; }

        bool Matches(string command);
        void Parse(string[] args);
        bool IsValid();
        int Execute();
        void CommandHelp();
    }
}