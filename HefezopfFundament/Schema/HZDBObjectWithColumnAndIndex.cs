using Gsaelzbrot.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Basetype for table, view and table type.
    /// </summary>
    public class HZDBObjectWithColumnAndIndex : HZDBObjectWithColumn {
        protected readonly HZDBOwnedCollection<HZDBIndex, HZDBObjectWithColumnAndIndex> _HZIndexes;
        protected readonly HZCastingList<IGsbIndex, HZDBIndex> _GsbIndexes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBObjectWithColumnAndIndex"/> class.
        /// </summary>
        public HZDBObjectWithColumnAndIndex() {
            this._HZIndexes = new HZDBOwnedCollection<HZDBIndex, HZDBObjectWithColumnAndIndex>(this);
            this._GsbIndexes = new HZCastingList<IGsbIndex, HZDBIndex>(this._HZIndexes, (gsb) => (HZDBIndex)gsb, (hz) => (IGsbIndex)hz);
        }

        public ICollection<HZDBIndex> Indexes {
            get {
                return this._HZIndexes;
            }
            set {
                SetterListProperty(this._HZIndexes, value);
            }
        }

        public HZDBIndex AddIndex(string name) {
            var result = new HZDBIndex();
            result.Name = name;
            this.Indexes.Add(result);
            return result;
        }
    }
}
