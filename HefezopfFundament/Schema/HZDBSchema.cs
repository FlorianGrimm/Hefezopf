using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    public class HZDBSchema : HZNamed
    {
        //private readonly List<INamed> _Named;
        private readonly HZDBOwnedCollection<HZDBSynonym, HZDBSchema> _Synonyms;
        private readonly HZDBOwnedCollection<HZDBTable, HZDBSchema> _Tables;
        private readonly HZDBOwnedCollection<HZDBView, HZDBSchema> _Views;
        private readonly HZDBOwnedCollection<HZDBStoredProcedure, HZDBSchema> _StoredProcedures;

        private readonly HZDBOwnedCollection<HZDBFuntion, HZDBSchema> _Funtions;
        private readonly HZDBOwnedCollection<HZDBUserDefinedType, HZDBSchema> _UserDefinedTypes;
        private readonly HZDBOwnedCollection<HZDBTableType, HZDBSchema> _TableType;

        public HZDBSchema()
        {
            this._Synonyms = new HZDBOwnedCollection<HZDBSynonym, HZDBSchema>(this);
            this._Tables = new HZDBOwnedCollection<HZDBTable, HZDBSchema>(this);
            this._Views = new HZDBOwnedCollection<HZDBView, HZDBSchema>(this);
            this._StoredProcedures = new HZDBOwnedCollection<HZDBStoredProcedure, HZDBSchema>(this);
            this._Funtions = new HZDBOwnedCollection<HZDBFuntion, HZDBSchema>(this);
            this._UserDefinedTypes = new HZDBOwnedCollection<HZDBUserDefinedType, HZDBSchema>(this);
            this._TableType = new HZDBOwnedCollection<HZDBTableType, HZDBSchema>(this);
        }

        public Collection<HZDBSynonym> Synonyms { get { return this._Synonyms; } set { SetterListProperty(this._Synonyms, value); } }
        public Collection<HZDBTable> Tables { get { return this._Tables; } set { SetterListProperty(this._Tables, value); } }
        public Collection<HZDBView> Views { get { return this._Views; } set { SetterListProperty(this._Views, value); } }
        public Collection<HZDBStoredProcedure> StoredProcedures { get { return this._StoredProcedures; } set { SetterListProperty(this._StoredProcedures, value); } }
        public Collection<HZDBFuntion> Funtions { get { return this._Funtions; } set { SetterListProperty(this._Funtions, value); } }
        public Collection<HZDBUserDefinedType> UserDefinedTypes { get { return this._UserDefinedTypes; } set { SetterListProperty(this._UserDefinedTypes, value); } }
        public Collection<HZDBTableType> TableType { get { return this._TableType; } set { SetterListProperty(this._TableType, value); } }

        public HZDBTable NewDBTable(string name)
        {
            var result = new HZDBTable();
            result.Name = name;
            this.Tables.Add(result);
            return result;
        }
    }

    public class HZDBSchemaOwned : HZNamed, IHZDBOwned<HZDBSchema>
    {
        private HZDBSchema _Schema;

        public HZDBSchemaOwned()
        {
        }

        public HZDBSchema Schema
        {
            get
            {
                return this._Schema;
            }
        }
        public void SetOwner(HZDBSchema owner)
        {
            this._Schema = owner;
        }
    }
}
