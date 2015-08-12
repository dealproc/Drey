using System.IO;

namespace Drey.Server.Models
{
    public class FileDownload
    {
        public string MimeType { get; set; }
        public string Filename { get; set; }
        public Stream FileContents { get; set; }
    }
}
