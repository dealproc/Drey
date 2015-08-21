using System;

namespace Drey.Nut
{
    [Serializable]
    public class ShellRequestArgs : EventArgs
    {
        public ShellAction ActionToTake { get; set; }
        public INutConfiguration ConfigurationManager { get; set; }
        public string PackageId { get; set; }
        public string Version { get; set; }
    }

    [Flags]
    public enum ShellAction
    {
        Startup = 1,
        Shutdown = 2,
        Restart = Startup & Shutdown
    }
}