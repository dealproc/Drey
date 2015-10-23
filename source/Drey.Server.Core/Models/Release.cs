using NuGet;

using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Server.Models
{
    public class Release
    {
        /// <summary>
        /// Gets or sets the nuget package id.
        /// </summary>
        [Key]
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the version of this package.
        /// </summary>
        [Key]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the description, as set in the nuspec manifest.
        /// <remarks>This field can contain markdown</remarks>
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the icon URL, as set in the nuspec manifest.
        /// </summary>
        public Uri IconUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Release"/> is listed.
        /// </summary>
        public bool Listed { get; set; }
        
        /// <summary>
        /// Gets or sets the published date/time with timezone information.
        /// </summary>
        public DateTimeOffset Published { get; set; }
        
        /// <summary>
        /// Gets or sets the release notes, as set in the nuspec manifest.
        /// <remarks>This field can contain markdown</remarks>
        /// </summary>
        public string ReleaseNotes { get; set; }
        
        /// <summary>
        /// Gets or sets the summary, as set in the nuspec manifest.
        /// <remarks>This field can contain markdown</remarks>
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// Gets or sets the tags, as set in the nuspec manifest.
        /// </summary>
        public string Tags { get; set; }
        
        /// <summary>
        /// Gets or sets the title, as set in the nuspec manifest.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the SHA1 Hash of the file contents for this release.
        /// <remarks>In the event of a re-publish of a package, if the manifest has the same id/version, but the hashes do not match, we want to update the stored package with the new package.</remarks>
        /// </summary>
        public string SHA1 { get; set; }

        /// <summary>
        /// Gets or sets the relative URI of this package.
        /// <remarks>This will be passed to an <see cref="IFileService"/> to retrieve the contents of the file.</remarks>
        /// </summary>
        public string RelativeUri { get; set; }
    }
}