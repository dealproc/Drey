using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    /// <summary>
    /// Represents a release managed in the Drey Runtime
    /// </summary>
    public class Release : DataModelBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the icon URL.
        /// </summary>
        public string IconUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Release"/> is listed.
        /// </summary>
        public bool Listed { get; set; }
        
        /// <summary>
        /// Gets or sets the published.
        /// </summary>
        public DateTime Published { get; set; }
        
        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
        public string ReleaseNotes { get; set; }
        
        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public string Tags { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the SHA of the file to be downloaded.
        /// </summary>
        [StringLength(40)]
        public string SHA1 { get; set; }
    }
}