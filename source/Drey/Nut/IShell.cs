using System;
using System.Threading.Tasks;
namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        string PackageId { get; }
        string InstanceId { get; }
        Task Shutdown();
    }
}