    namespace Hefezopf.Contracts.Host {

        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;

        /// <summary>
        /// 
        /// </summary>
        public class HZServer {
            /// <summary>
            /// 
            /// </summary>
            public HZServer() {
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
