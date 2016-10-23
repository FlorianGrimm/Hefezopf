using Gsaelzbrot.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Basetype for table, view....
    /// </summary>
    public class HZDBObjectWithColumn
        : HZDBSchemaOwned {
        protected readonly HZDBOwnedCollection<HZDBColumn, HZDBObjectWithColumn> _HZColumns;
        protected readonly HZCastingList<IGsbColumn, HZDBColumn> _GsbColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBObjectWithColumn"/> class.
        /// </summary>
        public HZDBObjectWithColumn() {
            this._HZColumns = new HZDBOwnedCollection<HZDBColumn, HZDBObjectWithColumn>(this);
            this._GsbColumns = new HZCastingList<IGsbColumn, HZDBColumn>(this._HZColumns, (gsb) => (HZDBColumn)gsb, (hz) => (IGsbColumn)hz);
        }

        public IList<HZDBColumn> Columns {
            get {
                return this._HZColumns;
            }
            set {
                SetterListProperty(this._HZColumns, value);
            }
        }

        public HZDBColumn AddColumn(string name) {
            //this.Database.Factory.CreateGsbSchema
            var result = new HZDBColumn();
            result.Database = this.Database;
            result.Name = name;
            this.Columns.Add(result);
            return result;
        }
    }
}
