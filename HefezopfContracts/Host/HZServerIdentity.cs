namespace Hefezopf.Contracts.Host {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class HZServerIdentity {
        /// <summary>
        /// 
        /// </summary>
        public HZServerIdentity() {
        }

        /// <summary>
        /// the maschine Name
        /// </summary>
        public string MaschineName { get; set; }

        /// <summary>
        /// the maschine sid.
        /// </summary>
        public string LocalSid { get; set; }
    }
}
