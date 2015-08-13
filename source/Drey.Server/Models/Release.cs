namespace Drey.Server.Models
{
    public class Release
    {
        public string SHA1 { get; set; }
        public string Filename { get; set; }
        public long Filesize { get; set; }
        public int Ordinal { get; set; }
    }
}