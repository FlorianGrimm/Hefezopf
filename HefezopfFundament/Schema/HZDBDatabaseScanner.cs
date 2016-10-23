namespace Hefezopf.Fundament.Schema {
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
        : IGsbModelFactory {
        private readonly HZDatabase _Database;
        private readonly IGsaelzbrot _Gsaelzbrot;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDatabaseScanner"/> class.
        /// </summary>
        /// <param name="serverInstance">The server optional nstance</param>
        /// <param name="databaseName">The database name</param>
        /// <param name="modelFactory">The model factory - can be null.</param>
        /// <param name="database">the target database - can be null.</param>
        public HZDBDatabaseScanner(string serverInstance, string databaseName, IGsbModelFactory modelFactory, HZDatabase database) {
            if (database == null) {
                database = new HZDatabase();
            }
            this._Database = database;
            this._Gsaelzbrot = global::Gsaelzbrot.Library.Factory.Instance.GsaelzbrotConnection(serverInstance, databaseName, modelFactory ?? this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDatabaseScanner"/> class.
        /// </summary>
        /// <param name="gsaelzbrot">The Gsaelzbrot</param>
        /// <param name="database">the target database - can be null.</param>
        public HZDBDatabaseScanner(IGsaelzbrot gsaelzbrot, HZDatabase database) {
            if (database == null) {
                database = new HZDatabase();
            }
            this._Database = database;
            this._Gsaelzbrot = gsaelzbrot;
        }

        /// <summary>
        /// Scan the database.
        /// </summary>
        /// <returns>this.</returns>
        public HZDatabase Scan() {
            this.ScanSchema();
            return this._Database;
        }

        /// <summary>
        /// Scan the datbase for schemas.
        /// </summary>
        /// <returns>this.</returns>
        public HZDatabase ScanSchema() {
            var schemas = this._Gsaelzbrot.GetSchemas();
            if (schemas != null) {
                foreach (IGsbSchema schema in schemas) {
                    this._Database.Schemas.Add((HZDBSchema)schema);
                }
            }
            return this._Database;
        }

        /// <summary>
        /// Creates a schema.
        /// </summary>
        /// <returns>a new schema.</returns>
        public IGsbSchema CreateGsbSchema() {
            return new HZDBSchema();
        }

        public IGsbTable CreateGsbTable() {
            return new HZDBTable();
            throw new NotImplementedException();
        }

        public IGsbColumn CreateGsbColumn() {
            throw new NotImplementedException();
        }

        public IGsbDataType CreateGsbDataType() {
            throw new NotImplementedException();
        }

        public IGsbIndex CreateGsbIndex() {
            throw new NotImplementedException();
        }

        public IGsbIndexedColumn CreateGsbIndexedColumn() {
            throw new NotImplementedException();
        }

        public IGsbView CreateGsbView() {
            throw new NotImplementedException();
        }

        public IGsbStoredProcedure CreateGsbStoredProcedure() {
            throw new NotImplementedException();
        }

        public IGsbFunction CreateGsbFunction() {
            throw new NotImplementedException();
        }

        public IGsbParameter CreateGsbParameter() {
            throw new NotImplementedException();
        }
    }
}
