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
        private IGsbModelFactory _Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDatabase"/> class.
        /// </summary>
        public HZDatabase() {
            this._Schemas = new List<HZDBSchema>();
        }

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
        public List<HZDBSchema> Schemas {
            get {
                return this._Schemas;
            }
            set {
                if (ReferenceEquals(this._Schemas, value)) { return; }
                this._Schemas.Clear();
                if (ReferenceEquals(null, value)) { return; }
                this._Schemas.AddRange(value);
            }
        }

        public HZDBSchema GetSchema(string name) {
            return this._Schemas.FirstOrDefault(schema => string.Equals(schema.Name, name, StringComparison.OrdinalIgnoreCase));
        }
        public HZDBSchema AddSchema(string name) {
            var result = this._Schemas.FirstOrDefault(schema => string.Equals(schema.Name, name, StringComparison.OrdinalIgnoreCase));
            if (result == null) {
                result = new HZDBSchema();
                result.Database = this.Database;
                result.Name = name;
                this._Schemas.Add(result);
            }
            return result;
        }
    }
}
