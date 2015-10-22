using System.IO;
namespace Drey
{
    public static class DreyConstants
    {
        public static string ConfigurationPackageName { get { return "drey.configuration"; } }
        public static string RelativeUrlMarker { get { return "~" + Path.DirectorySeparatorChar.ToString(); } }
    }
}