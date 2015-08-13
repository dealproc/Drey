namespace Drey.Nut
{
    public interface IGlobalSettings
    {
        string this[string key] { get; }
    }
}