using Gsaelzbrot.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represent a SQL Table
    /// </summary>
    public class HZDBTable
        : HZDBObjectWithColumnAndIndex
        , IGsbTable {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBTable"/> class.
        /// </summary>
        public HZDBTable() {
        }

        IList<IGsbColumn> IGsbTable.Columns {
            get {
                return this._GsbColumns;
            }

            set {
                SetterListProperty(this._GsbColumns, value);
            }
        }

        IList<IGsbIndex> IGsbTable.Indexes {
            get {
                return this._GsbIndexes;
            }

            set {
                SetterListProperty(this._GsbIndexes, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        string IGsbSchemaNamed.Schema {
            get {
                return this.Schema?.Name;
            }

            set {
                this.SetOwner(this.Database.AddSchema(value));
            }
        }

        protected override void SetSchema(HZDBSchema oldSchema, HZDBSchema newSchema) {
            SetSchemaForThat(oldSchema, newSchema, this, (schema) => schema.Tables);
            base.SetSchema(oldSchema, newSchema);
        }
    }
}
