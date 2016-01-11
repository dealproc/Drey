namespace Drey.Configuration.DataModel
{
    /// <summary>
    /// A package (distinct list selection of Id, Title from release.)
    /// <remarks>Need to validate if the AutoUpdates flag is still relevant</remarks>
    /// </summary>
    public class Package
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Determines whether or not the package in question is setup to auto-update from a remote feed.
        /// </summary>
        public bool AutoUpdates { get; set; }
    }
}