namespace Hefezopf.Fundament.Schema
{
    using Gsaelzbrot.Library;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Scan a database
    /// </summary>
    public class HZDBDatabaseScanner
        : GsbDatabaseScanner {
        private readonly HZDatabase _Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDatabaseScanner"/> class.
        /// </summary>
        /// <param name="serverInstance">The server optional nstance</param>
        /// <param name="databaseName">The database name</param>
        /// <param name="modelFactory">The model factory - can be null.</param>
        /// <param name="database">the target database - can be null.</param>
        public HZDBDatabaseScanner(string serverInstance, string databaseName, IGsbModelFactory modelFactory, HZDatabase database)
            : base(global::Gsaelzbrot.Library.Factory.Instance.GsaelzbrotConnection(serverInstance, databaseName, modelFactory ?? new HZDBModelFactory()))
        {
            if (database == null)
            {
                database = new HZDatabase();
            }
            this._Database = database;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDatabaseScanner"/> class.
        /// </summary>
        /// <param name="gsaelzbrot">The Gsaelzbrot</param>
        /// <param name="database">the target database - can be null.</param>
        public HZDBDatabaseScanner(IGsaelzbrot gsaelzbrot, HZDatabase database)
            : base(gsaelzbrot)
        {
            if (database == null)
            {
                database = new HZDatabase();
            }
            this._Database = database;
        }

        protected override void AddSchema(IGsbSchema schema)
        {
            //base.AddSchema(schema);
            if (this._Database.GetSchema(schema.Name) == null)
            {
                this._Database.Schemas.Add((HZDBSchema)schema);
            }
        }

        protected override void AddTable(GsbTable table)
        {
            //base.AddTable(table);
            if (this._Database.GetTable(table.Schema, table.Name) == null)
            {
                this._Database.Tables.Add((HZDBTable)table);
            }
        }

        protected override void AddView(GsbView view)
        {
            //base.AddView(view);
            if (this._Database.GetView(view.Schema, view.Name) == null)
            {
                this._Database.Views.Add((HZDBView)view);
            }
        }

        
        /*
        /// <returns>this.</returns>
        public HZDatabase Scan()
        {
            this.ScanSchema();
            this.ScanTable();
            return this._Database;
        }

        /// <returns>this.</returns>
        public HZDatabase ScanSchema()
        {
            var schemas = this._Gsaelzbrot.GetSchemas();
            if (schemas != null)
            {
                foreach (IGsbSchema schema in schemas)
                {
                    if (this._Database.GetSchema(schema.Name) == null)
                    {
                        this._Database.Schemas.Add((HZDBSchema)schema);
                    }
                }
            }
            return this._Database;
        }

        /// <returns>The target.</returns>
        public HZDatabase ScanTable()
        {
            var tables = this._Gsaelzbrot.GetTables();
            if (tables != null)
            {
                foreach (IGsbTable gsbTable in tables)
                {
                    var hzTable = gsbTable as HZDBTable;
                    if (hzTable != null)
                    {
                        this._Database.AddTable(hzTable);
                    }
                    else
                    {
                        //var schema = this._Database.AddSchema(gsbTable.Schema);
                        //hzTable = this._Database.GetTable(gsbTable.Schema, gsbTable.Name);
                        //if (hzTable == null)
                        //{
                        //    hzTable = this._Database.AddTable(gsbTable.Schema, gsbTable.Name);
                        //}
                        //else { }
                        //foreach (var gsbColumn in gsbTable.Columns)
                        //{
                        //    //hzTable.AddColumn();
                        //}
                        throw new NotImplementedException("TODO");
                    }
                }
            }
            return this._Database;
        }
        */

    }
}
