﻿namespace Hefezopf.Contracts.Host {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    [Guid("9B10A98B-87A6-4085-BA3D-D6AE1229F4E7")]
    public class HZHost {
        public  HZHostIdentity HostIdentity { get; set; }
        /// <summary>
        /// The HostType
        /// </summary>
        public string HostType { get; set; }
        
        /// <summary>
        /// the server identity
        /// </summary>
        public HZServerIdentity ServerId { get; set; }

        /// <summary>
        /// the server
        /// </summary>
        public HZServer Server { get; set; }
    }
}
