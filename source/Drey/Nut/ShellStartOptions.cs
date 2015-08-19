using System;
namespace Drey.Nut
{
    [Serializable]
    public class ShellStartOptions
    {
        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        public string PackageId { get; set; }
        
        /// <summary>
        /// Gets or sets the display description of the package.
        /// </summary>
        public string DisplayAs { get; set; }
        
        /// <summary>
        /// Gets or sets the full path to access the entryPoint dll.
        /// </summary>
        public string DllPath { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        public string AssemblyName { get; set; }
        
        /// <summary>
        /// Gets or sets the startup class full description 
        /// <para>e.g. typeof({class}).FullName</para>
        /// </summary>
        public string StartupClass { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the application domain.
        /// </summary>
        public string ApplicationDomainName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [provide configuration options].
        /// </summary>
        public bool ProvideConfigurationOptions { get; set; }
    }
}