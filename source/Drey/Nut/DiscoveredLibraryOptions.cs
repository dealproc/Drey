using System;

namespace Drey.Nut
{
    [Serializable]
    public class DiscoveredLibraryOptions
    {
        /// <summary>
        /// The full filename and path for the entry dll.
        /// </summary>
        public string File { get; private set; }

        /// <summary>
        /// The Type.FullName value for the shell implementation type to be instantiated.
        /// </summary>
        public string TypeFullName { get; private set; }

        /// <summary>
        /// The package id for the shell to be instantiated.
        /// </summary>
        public string PackageId { get; private set; }

        public DiscoveredLibraryOptions(string file, string typeFullName, string packageId)
        {
            File = file;
            TypeFullName = typeFullName;
            PackageId = packageId;
        }
    }
}
