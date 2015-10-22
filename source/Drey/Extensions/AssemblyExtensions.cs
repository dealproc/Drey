using System.IO;
using System.Reflection;

namespace Drey.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetDirectoryLocation(this Assembly asm)
        {
            var fullDirectoryPath = asm.CodeBase;
            return Path.GetDirectoryName(fullDirectoryPath.Remove(0, 8)) + Path.DirectorySeparatorChar.ToString();
        }
    }
}
