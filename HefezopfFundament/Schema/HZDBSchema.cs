using Gsaelzbrot.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represent a schema
    /// </summary>
    public class HZDBSchema : HZNamed, IGsbSchema {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBSchema"/> class.
        /// </summary>
        public HZDBSchema() {
        }

        public HZDBTable AddTable(string name, HZDatabase database)
        {
            return database.AddTable(this.Name, name);
        }
    }
}
