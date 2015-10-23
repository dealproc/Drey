using System;
using System.IO;
using System.Security.Cryptography;

namespace Drey.Server
{
    public static class Utilities
    {
        public static string CalculateChecksum(Stream stream)
        {
            SHA1Managed sha = new SHA1Managed();
            byte[] checksum = sha.ComputeHash(stream);
            string checksumStr = BitConverter.ToString(checksum).Replace("-", string.Empty).ToUpper();
            return checksumStr;
        }
    }
}
