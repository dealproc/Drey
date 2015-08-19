namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IGlobalSettings GlobalSettings { get; }
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string HordeBaseDirectory { get; }
    }
}