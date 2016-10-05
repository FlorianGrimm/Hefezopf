using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBObjectWithColumn : HZDBSchemaOwned
    {
        private readonly List<HZDBColumn> _Columns;
        private HZDBSchema _Schema;

        public HZDBObjectWithColumn()
        {
            this._Columns = new List<HZDBColumn>();
        }

        public List<HZDBColumn> Columns
        {
            get
            {
                return this._Columns;
            }
            set
            {
                SetterListProperty(this._Columns, value);
            }
        }

        public HZDBColumn AddColumn(string name)
        {
            var result = new HZDBColumn();
            result.Name = name;
            this.Columns.Add(result);
            return result;
        }
    }
}
