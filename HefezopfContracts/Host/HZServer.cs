namespace Hefezopf.Contracts.Host {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO
    /// </summary>
    public class HZServer {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZServer"/> class.
        /// </summary>
        public HZServer() {
        }

        /// <summary>
        /// Gets or sets the maschine Name
        /// </summary>
        public string MaschineName { get; set; }

        /// <summary>
        /// Gets or sets the maschine sid.
        /// </summary>
        public string LocalSid { get; set; }
    }
}
