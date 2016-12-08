namespace Hefezopf.Library.Schema {
    using Gsaelzbrot.Library;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The Hefezopf Configuration
    /// </summary>
    public class HefezopfConfiguration {
        /// <summary>The target.</summary>
        public readonly GsbDatabase DBHefezopf;

        /// <summary>Hefezops</summary>
        public readonly GsbSchema SchemaHefezopf;

        /// <summary>ZomieSate</summary>
        public readonly GsbTable Hefezopf_ZombieState;

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfConfiguration"/> class.
        /// </summary>
        public HefezopfConfiguration() {
            this.DBHefezopf = new GsbDatabase();
            this.SchemaHefezopf = this.DBHefezopf.AddSchema("Hefezopf");
            this.Hefezopf_ZombieState = this.SchemaHefezopf.AddTable("ZombieState");
            this.Hefezopf_ZombieState = this.DBHefezopf.AddTable(this.SchemaHefezopf.Name, "ZombieState");
            this.Hefezopf_ZombieState.AddColumn("Name");
        }
    }
}
