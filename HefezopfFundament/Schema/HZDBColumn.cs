using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBColumn : HZNamed,IHZDBOwned<HZDBObjectWithColumnAndIndex>
    {
        private HZDBObjectWithColumnAndIndex _Owner;

        public HZDBColumn()
        {
        }

        public HZDBObjectWithColumnAndIndex Owner => this._Owner;

        public HZDBDataType DataType { get; set; }

        public bool NotNULL { get; set; }

        public string Collaction { get; set; }

        public void SetOwner(HZDBObjectWithColumnAndIndex owner)
        {
            this._Owner = owner;
        }

        public HZDBColumn SetType(System.Data.SqlDbType sqlDbType)
        {
            this.DataType = HZDBDataType.From(sqlDbType);
            throw new NotImplementedException();
        }
    }
}
