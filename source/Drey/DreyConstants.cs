using System.IO;
namespace Drey
{
    public static class DreyConstants
    {
        /// <summary>
        /// The Id of the console package.  Used for loading the console, etc.
        /// </summary>
        public static string ConfigurationPackageName { get { return "Drey.Configuration"; } }
        /// <summary>
        /// The string representation of a relative url path.  Used with a "{some string path}".StartsWith(DreyConstants.RelativeUrlMarker); to determine if the path is relative or absolute.
        /// </summary>
        public static string RelativeUrlMarker { get { return "~" + Path.DirectorySeparatorChar.ToString(); } }
        /// <summary>
        /// Key name for the Server Hostname stored in the global settings sqlite database.
        /// </summary>
        public static string ServerHostname { get { return "ServerHostname"; } }
        /// <summary>
        /// Key name for the Client Certificate stored in the global settings sqlite database.
        /// </summary>
        public static string ClientCertificate { get { return "SSLPfx"; } }
        /// <summary>
        /// Key name for the SSL Thumbprint stored in the global settings sqlite database.
        /// </summary>
        public static string ServerSSLThumbprint { get { return "server.sslthumbprint"; } }
    }
}