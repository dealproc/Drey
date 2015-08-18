using System;
using System.Threading.Tasks;
namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string PackageId { get; }
        Task Shutdown();
    }
}