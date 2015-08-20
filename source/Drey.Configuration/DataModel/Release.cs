using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    public class Release : DataModelBase
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public Uri IconUrl { get; set; }
        public bool Listed { get; set; }
        public DateTime Published { get; set; }
        public string ReleaseNotes { get; set; }
        public string Summary { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the SHA of the file to be downloaded.
        /// </summary>
        [StringLength(40)]
        public string SHA1 { get; set; }

        /// <summary>
        /// Gets or sets the filename for this release.
        /// </summary>
        [StringLength(255)]
        public string Filename { get; set; }
    }
}