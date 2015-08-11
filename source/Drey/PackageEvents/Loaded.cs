using System;

namespace Drey.PackageEvents
{
    [Serializable]
    public class Loaded
    {
        public string PackageId { get; set; }
        public string InstanceId { get; set; }
    }
}