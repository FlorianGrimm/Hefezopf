using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBObjectWithColumnAndIndex : HZDBObjectWithColumn
    {
        private readonly HZDBOwnedCollection<HZDBIndex, HZDBObjectWithColumnAndIndex> _Indexes;

        public HZDBObjectWithColumnAndIndex()
        {
            this._Indexes = new HZDBOwnedCollection<HZDBIndex, HZDBObjectWithColumnAndIndex>(this);
        }

        public ICollection<HZDBIndex> Indexes
        {
            get
            {
                return this._Indexes;
            }
            set
            {
                SetterListProperty(this._Indexes, value);
            }
        }

        public HZDBIndex AddIndex(string name)
        {
            var result = new HZDBIndex();
            result.Name = name;
            this.Indexes.Add(result);
            return result;
        }
    }
}
