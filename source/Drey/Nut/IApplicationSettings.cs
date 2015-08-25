using System.Collections.Generic;
namespace Drey.Nut
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
        void Register(IEnumerable<string> keys);
    }
}