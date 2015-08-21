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
        void Startup(INutConfiguration configurationManager);
        Task Shutdown();
    }
}