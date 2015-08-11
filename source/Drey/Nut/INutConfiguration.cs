namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IPackageEventBus EventBus { get; }
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string HordeBaseDirectory { get; }
    }
}