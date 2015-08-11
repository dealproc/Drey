using System;
using System.Threading.Tasks;
namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string InstanceId { get; }
        Task Shutdown();
    }
}