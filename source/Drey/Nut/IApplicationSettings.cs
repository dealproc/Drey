using System.Collections.Generic;
namespace Drey.Nut
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
        bool Exists(string key);
        void Register(string key, string value = "");
    }
}