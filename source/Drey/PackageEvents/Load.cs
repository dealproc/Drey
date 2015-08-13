using System;

namespace Drey.PackageEvents
{
    [Serializable]
    public class Load
    {
        public string PackageId { get; set; }
        public Drey.Nut.INutConfiguration ConfigurationManager { get; set; }
    }
}