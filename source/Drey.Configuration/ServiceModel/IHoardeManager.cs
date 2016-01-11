using System;
namespace Drey.Configuration.ServiceModel
{
    /// <summary>
    /// Interface for the Hoarde Manager
    /// </summary>
    public interface IHoardeManager : IDisposable
    {
        /// <summary>
        /// Handles a ShellRequestArgs event.
        /// </summary>
        /// <param name="e">The e.</param>
        void Handle(Drey.Nut.ShellRequestArgs e);

        /// <summary>
        /// Determines whether the specified package is loaded and executing in the runtime.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        bool IsOnline(DataModel.Release package);
    }
}
