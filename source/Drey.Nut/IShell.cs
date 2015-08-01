using System;
namespace Drey.Nut
{
    public interface IShell : IDisposable
    {
        event EventHandler<ShellEventArgs> ShellCallback;
    }
}
