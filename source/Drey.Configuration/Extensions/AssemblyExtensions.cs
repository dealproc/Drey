using System.Reflection;

namespace Drey.Configuration.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetDirectoryLocation(this Assembly asm)
        {
            var fullDirectoryPath = asm.CodeBase;
            return fullDirectoryPath.Remove(0, 8);
        }
    }
}
