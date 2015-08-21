using System;
using System.Threading.Tasks;

namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string Id { get; }
        string NameDomainAs { get; }
        string DisplayAs { get; }
        bool RequiresConfigurationStorage { get; }

        event EventHandler<ShellRequestArgs> OnShellRequest;

        void Startup(INutConfiguration configurationManager);
        void Shutdown();
    }
}