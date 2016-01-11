using System;

namespace Drey.Configuration.Services.ViewModels
{
    public class AppletInfoPmo
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
        /// Gets or sets a value indicating whether this <see cref="AppletInfoPmo"/> is online.
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the published.
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the SHA of the file to be downloaded.
        /// </summary>
        public string SHA1 { get; set; }
    }
}
