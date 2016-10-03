using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HefezopfLibrary.Schema
{
    public class HZDatabaseModel
    {
        private readonly List<HZDBSchema> _Schemas;
        public HZDatabaseModel()
        {
            this._Schemas = new List<HZDBSchema>();
        }

        public string Name { get; set; }

        public List<HZDBSchema> Schemas
        {
            get
            {
                return this._Schemas;
            }
            set
            {
                if (ReferenceEquals(this._Schemas, value)) { return; }
                this._Schemas.Clear();
                if (ReferenceEquals(null, value)) { return; }
                this._Schemas.AddRange(value);
            }
        }
    }
}
