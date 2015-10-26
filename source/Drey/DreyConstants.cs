using System.IO;
namespace Drey
{
    public static class DreyConstants
    {
        public static string ConfigurationPackageName { get { return "drey.configuration"; } }
        public static string RelativeUrlMarker { get { return "~" + Path.DirectorySeparatorChar.ToString(); } }
        public static string ServerHostname { get { return "ServerHostname"; } }
        public static string ClientCertificate { get { return "SSLPfx"; } }
    }
}