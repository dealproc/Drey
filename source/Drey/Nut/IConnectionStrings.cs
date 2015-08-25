using System.Collections.Generic;
namespace Drey.Nut
{
    public interface IConnectionStrings
    {
        string this[string key] { get; }
        void Register(IEnumerable<string> keys);
    }
}