namespace Drey.Nut
{
    public interface IConnectionStrings
    {
        string this[string key] { get; }
    }
}