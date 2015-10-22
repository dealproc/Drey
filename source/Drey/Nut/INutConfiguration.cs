namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IGlobalSettings GlobalSettings { get; }
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string WorkingDirectory { get; }
        string HordeBaseDirectory { get; }
        string LogsDirectory { get; }
        ExecutionMode Mode { get; }
    }
}