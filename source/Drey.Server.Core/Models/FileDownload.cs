using System.IO;

namespace Drey.Server.Models
{
    public class FileDownload
    {
        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        public string MimeType { get; set; }
        
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; }
        
        /// <summary>
        /// Gets or sets the file contents.
        /// </summary>
        public Stream FileContents { get; set; }
    }
}