namespace Hefezopf.Fundament.Host {
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides informations of the computer.
    /// </summary>
    public class ComputerInformation {
        /// <summary>
        /// Get the local maschine SID.
        /// </summary>
        /// <returns>the local maschine SID.</returns>
        public static SecurityIdentifier GetLocalComputerSid() {
            const string path = "WinNT://.,Computer";
            var objectSID = (byte[])new DirectoryEntry(path).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID");
            return new SecurityIdentifier(objectSID, 0).AccountDomainSid;
        }
    }
}
