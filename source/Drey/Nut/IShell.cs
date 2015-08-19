using System;
using System.Threading.Tasks;
namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string Description { get; }
        string PackageId { get; }
        Task Shutdown();
    }
}