using System;

namespace Drey.PackageEvents
{
    [Serializable]
    public class Disposed
    {
        public string PackageId { get; set; }
    }
}