namespace Hefezopf.Fundament.Schema {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Owned by the Datbase
    /// </summary>
    public class HZDatabaseOwned {
        private HZDatabase _Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDatabaseOwned"/> class.
        /// </summary>
        public HZDatabaseOwned() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDatabaseOwned"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public HZDatabaseOwned(HZDatabase database) {
            this._Database = database;
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public HZDatabase Database {
            get {
                return this._Database;
            }
            set {
                if (ReferenceEquals(this._Database, value)) { return; }
                var oldValue = this._Database;
                this._Database = value;
                this.SetDatabase(oldValue, value);
            }
        }

        /// <summary>
        /// Set the database.
        /// </summary>
        /// <param name="oldValue">the old database.</param>
        /// <param name="value">the new database.</param>
        protected virtual void SetDatabase(HZDatabase oldValue, HZDatabase value) {
        }
    }
}
