using System.Collections.Generic;
namespace Drey.Nut
{
    public interface IConnectionStrings
    {
        string this[string key] { get; }
        bool Exists(string name);
        void Register(string name, string connectionString, string providerName = "");
    }
}