namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IPackageEventBus EventBus { get; }
        IGlobalSettings GlobalSettings { get; }
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string HordeBaseDirectory { get; }
    }
}