using System;

namespace Drey.Nut
{
    [Flags]
    public enum ServerAuthenticationTypes
    {
        /// <summary>
        /// No Credentials.
        /// </summary>
        Anonymous = 0,
        /// <summary>
        /// Uses a Network Credential (Username/Password)
        /// </summary>
        NetworkCredential = 1,
        /// <summary>
        /// Uses a Client Certificate
        /// </summary>
        ClientCertificate = 2,
        /// <summary>
        /// Uses Hawk Authentication
        /// </summary>
        Hawk = 4
    }
}