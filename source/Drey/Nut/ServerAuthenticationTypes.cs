using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Nut
{
    [Flags]
    public enum ServerAuthenticationTypes
    {
        /// <summary>
        /// No Credentials.
        /// </summary>
        [Display(Name = "Anonymous")]
        Anonymous = 0,
        /// <summary>
        /// Uses a Network Credential (Username/Password)
        /// </summary>
        [Display(Name = "Network Credential")]
        NetworkCredential = 1,
        /// <summary>
        /// Uses a Client Certificate
        /// </summary>
        [Display(Name = "Client Certificate")]
        ClientCertificate = 2,
        /// <summary>
        /// Uses Hawk Authentication
        /// </summary>
        [Display(Name = "Hawk")]
        Hawk = 4
    }
}