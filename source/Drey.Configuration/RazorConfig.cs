using Nancy.ViewEngines.Razor;
using System.Collections.Generic;

namespace Drey.Configuration
{
    public class RazorConfig : IRazorConfiguration
    {
        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "Drey.Configuration";
            yield return "Nancy.Validation.DataAnnotations";
            yield return "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "Nancy.Validation";
            yield return "Nancy.ViewEngines";
            yield return "Nancy.ViewEngines.Razor";
            yield return "System.Collections.Generic";
            yield return "System.Linq";
        }
    }
}