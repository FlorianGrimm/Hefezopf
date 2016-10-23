namespace Hefezopf.Fundament.Schema {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HZDBColumn
        : HZNamed
        , IHZDBOwned<HZDBObjectWithColumn> {
        private HZDBObjectWithColumn _Owner;

        public HZDBColumn() {
        }

        public HZDBObjectWithColumn Owner => this._Owner;

        public HZDBDataType DataType { get; set; }

        public bool NotNULL { get; set; }

        public string Collaction { get; set; }

        public void SetOwner(HZDBObjectWithColumn owner) {
            this._Owner = owner;
        }

        public HZDBColumn SetType(System.Data.SqlDbType sqlDbType) {
            this.DataType = HZDBDataType.From(sqlDbType);
            throw new NotImplementedException();
        }
    }
}
