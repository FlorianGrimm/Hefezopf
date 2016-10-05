using System;
using System.Collections.Generic;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBIndex : HZNamed, IHZDBOwned<HZDBObjectWithColumnAndIndex>
    {
        private readonly List<HZDBColumn> _Columns;

        public HZDBIndex()
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

        public void SetOwner(HZDBObjectWithColumnAndIndex owner)
        {
            throw new NotImplementedException();
        }
    }
}