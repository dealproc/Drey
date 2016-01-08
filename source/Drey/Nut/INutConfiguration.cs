using Drey.CertificateValidation;
namespace Drey.Nut
{
    public interface INutConfiguration
    {
        IGlobalSettings GlobalSettings { get; }
        IApplicationSettings ApplicationSettings { get; }
        IConnectionStrings ConnectionStrings { get; }
        string WorkingDirectory { get; }
        string HoardeBaseDirectory { get; }
        string LogsDirectory { get; }
        ExecutionMode Mode { get; }
        string LogVerbosity { get; }
        ICertificateValidation CertificateValidator { get; }
    }
}