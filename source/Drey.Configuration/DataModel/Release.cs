using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    public class Release : DataModelBase
    {
        /// <summary>
        /// Gets or sets the reference package.
        /// </summary>
        public RegisteredPackage Package { get; set; }

        /// <summary>
        /// Gets or sets the SHA of the file to be downloaded.
        /// </summary>
        [StringLength(40)]
        public string SHA { get; set; }

        /// <summary>
        /// Gets or sets the filename for this release.
        /// </summary>
        [StringLength(256)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the ordinal of the release (controls in which order the release is applied).
        /// </summary>
        public int Ordinal { get; set; }
    }
}