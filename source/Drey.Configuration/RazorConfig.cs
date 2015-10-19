using Drey.Logging;
using Nancy.ViewEngines.Razor;
using System.Collections.Generic;

namespace Drey.Configuration
{
    public class RazorConfig : IRazorConfiguration
    {
        static ILog _log = LogProvider.For<RazorConfig>();

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            _log.Trace("GetAssemblyNames() has been accessed.");

            yield return "Drey";
            yield return "Drey.Configuration";
            yield return "Nancy.Validation.DataAnnotations";
            yield return "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
        }

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