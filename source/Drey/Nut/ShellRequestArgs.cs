using System;

namespace Drey.Nut
{
    /// <summary>
    /// An eventArgs object expressing a command for the runtime to react to, from the drey.configuration applet.
    /// </summary>
    [Serializable]
    public class ShellRequestArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the action to take.
        /// </summary>
        public ShellAction ActionToTake { get; set; }
    
        /// <summary>
        /// Gets or sets the configuration manager.
        /// </summary>
        public INutConfiguration ConfigurationManager { get; set; }
    
        /// <summary>
        /// Gets or sets the package identifier.
        /// </summary>
        public string PackageId { get; set; }
    
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// During a restart operation, if this is set to true, then the system will go about cleaning up historical versions of the <see cref="PackageId"/> that exist within the hoarde.
        /// </summary>
        public bool RemoveOtherVersionsOnRestart { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellRequestArgs"/> class.
        /// </summary>
        public ShellRequestArgs()
        {
            RemoveOtherVersionsOnRestart = false;
        }
    }

    /// <summary>
    /// The possible actions that the runtime should take for the event.
    /// </summary>
    [Flags]
    public enum ShellAction
    {
        /// <summary>
        /// Command requests that the app instance be started within the runtime.
        /// </summary>
        Startup = 1,
        /// <summary>
        /// Command requests that the app instance be shutdown within the runtime.
        /// </summary>
        Shutdown = 2,
        /// <summary>
        /// Command requests that the app instance be restarted within the runtime.
        /// </summary>
        Restart = Startup & Shutdown,
        /// <summary>
        /// The heartbeat.
        /// </summary>
        Heartbeat = 4
    }
}