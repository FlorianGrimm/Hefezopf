using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Basetype for object that have a schema.
    /// </summary>
    public class HZDBSchemaOwned
        : HZNamed {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBSchemaOwned"/> class.
        /// </summary>
        public HZDBSchemaOwned() {
        }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public string Schema { get; set; }
    }
}
