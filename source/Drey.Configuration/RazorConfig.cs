using Drey.Logging;

using Nancy.ViewEngines.Razor;

using System.Collections.Generic;

namespace Drey.Configuration
{
    /// <summary>
    /// Razor configuration.
    /// </summary>
    public class RazorConfig : IRazorConfiguration
    {
        static ILog _log = LogProvider.For<RazorConfig>();

        /// <summary>
        /// Gets a value indicating whether to automatically include the model's namespace in the generated code.
        /// </summary>
        /// <value>
        /// <c>true</c> if the model's namespace should be automatically included in the generated code; otherwise, <c>false</c>.
        /// </value>
        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the assembly names for razor to use.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAssemblyNames()
        {
            _log.Trace("GetAssemblyNames() has been accessed.");

            yield return "Drey";
            yield return "Drey.Configuration";
            yield return "Nancy.Validation.DataAnnotations";
            yield return "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
        }

        /// <summary>
        /// Gets the default namespaces, for rendering Razor Views.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDefaultNamespaces()
        {
            _log.Trace("GetDefaultNamespaces() has been accessed.");

            yield return "Nancy.Validation";
            yield return "Nancy.ViewEngines";
            yield return "Nancy.ViewEngines.Razor";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
        }
    }
}