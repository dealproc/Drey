using System;
namespace Drey.Nut
{
    [Serializable]
    public class ShellStartOptions
    {
        public string PackageId { get; set; }
        public string DllPath { get; set; }
        public string AssemblyName { get; set; }
        public string StartupClass { get; set; }
        public string ApplicationDomainName { get; set; }
        public bool ProvideConfigurationOptions { get; set; }
    }
}