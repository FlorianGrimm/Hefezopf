namespace Hefezopf.Fundament.Schema {
    using Gsaelzbrot.Library;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The database with its object.
    /// </summary>
    public class HZDatabase : HZNamed {
        private readonly List<HZDBSchema> _Schemas;
        private readonly List<HZDBSynonym> _Synonyms;
        private readonly List<HZDBTable> _Tables;
        private readonly List<HZDBView> _Views;
        private readonly List<HZDBStoredProcedure> _StoredProcedures;

        private readonly List<HZDBFuntion> _Funtions;
        private readonly List<HZDBUserDefinedType> _UserDefinedTypes;
        private readonly List<HZDBTableType> _TableTypes;

        private IGsbModelFactory _Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDatabase"/> class.
        /// </summary>
        public HZDatabase() {
            this._Schemas = new List<HZDBSchema>();
            this._Synonyms = new List<HZDBSynonym>();
            this._Tables = new List<HZDBTable>();
            this._Views = new List<HZDBView>();
            this._StoredProcedures = new List<HZDBStoredProcedure>();
            this._Funtions = new List<HZDBFuntion>();
            this._UserDefinedTypes = new List<HZDBUserDefinedType>();
            this._TableTypes = new List<HZDBTableType>();
        }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        public IGsbModelFactory Factory {
            get {
                if (this._Factory == null) {
                    this._Factory = new HZDatabaseGsbModelFactory(this);
                }
                return this._Factory;
            }
            set {
                this._Factory = value;
            }
        }

        /// <summary>
        /// Gets or sets the schemas.
        /// </summary>
        public List<HZDBSchema> Schemas { get { return this._Schemas; } set { SetterListProperty(this._Schemas, value); } }

        /// <summary>
        /// Gets or sets the Synonyms.
        /// </summary>
        public List<HZDBSynonym> Synonyms { get { return this._Synonyms; } set { SetterListProperty(this._Synonyms, value); } }

        /// <summary>
        /// Gets or sets the Tables.
        /// </summary>
        public List<HZDBTable> Tables { get { return this._Tables; } set { SetterListProperty(this._Tables, value); } }

        /// <summary>
        /// Gets or sets the Views.
        /// </summary>
        public List<HZDBView> Views { get { return this._Views; } set { SetterListProperty(this._Views, value); } }

        /// <summary>
        /// Gets or sets the StoredProcedures.
        /// </summary>
        public List<HZDBStoredProcedure> StoredProcedures { get { return this._StoredProcedures; } set { SetterListProperty(this._StoredProcedures, value); } }

        /// <summary>
        /// Gets or sets the Funtions.
        /// </summary>
        public List<HZDBFuntion> Funtions { get { return this._Funtions; } set { SetterListProperty(this._Funtions, value); } }

        /// <summary>
        /// Gets or sets the UserDefinedTypes.
        /// </summary>
        public List<HZDBUserDefinedType> UserDefinedTypes { get { return this._UserDefinedTypes; } set { SetterListProperty(this._UserDefinedTypes, value); } }

        /// <summary>
        /// Gets or sets the TableTypes.
        /// </summary>
        public List<HZDBTableType> TableTypes { get { return this._TableTypes; } set { SetterListProperty(this._TableTypes, value); } }

        /// <summary>
        /// Get the Schema named as <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the schema</param>
        /// <returns>the schema - can be null.</returns>
        public HZDBSchema GetSchema(string name) {
            return this._Schemas.FirstOrDefault(schema => string.Equals(schema.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get or add the Schema named as <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the schema</param>
        /// <returns>the schema - not null.</returns>
        public HZDBSchema AddSchema(string name) {
            var result = this._Schemas.FirstOrDefault(schema => string.Equals(schema.Name, name, StringComparison.OrdinalIgnoreCase));
            if (result == null) {
                result = new HZDBSchema();
                result.Name = name;
                this._Schemas.Add(result);
            }
            return result;
        }

        /// <summary>
        /// Add a table.
        /// </summary>
        /// <param name="hzTable">the table to add</param>
        /// <exception cref="InvalidOperationException">A table with that name already exists.</exception>
        public void AddTable(HZDBTable hzTable) {
            var existing = this.GetTable(hzTable.Schema, hzTable.Name);
            if (existing == null) {
                this.Tables.Add(hzTable);
            } else if (ReferenceEquals(hzTable, existing)) {
                return;
            } else {
                throw new InvalidOperationException("Already exists.");
            }
        }

        /// <summary>
        /// Create and add a table.
        /// </summary>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="name">The name of the table.</param>
        /// <returns>a new table</returns>
        /// <exception cref="InvalidOperationException">A table with that name already exists.</exception>
        public HZDBTable AddTable(string schema, string name) {
            var result = new HZDBTable();
            result.Schema = schema;
            result.Name = name;
            this.AddTable(result);
            return result;
        }

        /// <summary>
        /// Find a table with that schema and name.
        /// </summary>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="name">The name of the table.</param>
        /// <returns>The table or null</returns>
        public HZDBTable GetTable(string schema, string name) {
            return GetByName(this.Tables, schema, name);
        }

        /// <summary>
        /// Find a view with that schema and name.
        /// </summary>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="name">The name of the view.</param>
        /// <returns>The view or null</returns>
        public HZDBView GetView(string schema, string name)
        {
            return GetByName(this.Views, schema, name);
        }

        private static T GetByName<T>(IEnumerable<T> list, string schema, string name)
            where T : HZDBSchemaOwned {
            return list.FirstOrDefault(_ => 
                string.Equals(_.Schema, schema, StringComparison.OrdinalIgnoreCase) 
                && string.Equals(_.Name, name, StringComparison.OrdinalIgnoreCase));
        }

    }
}
