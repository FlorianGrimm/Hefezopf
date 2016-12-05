using Gsaelzbrot.Library;
using System;
using System.Collections.Generic;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBIndex
        : HZNamed
        , GsbIndex {
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

        public string FilterDefinition {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool IgnoreDuplicateKeys {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public IList<GsbIndexedColumn> IndexedColumns {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool IsClustered {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool IsPrimary {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool IsUnique {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }
    }
}