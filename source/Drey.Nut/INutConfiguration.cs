namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string HordeBaseDirectory { get; }
    }
}