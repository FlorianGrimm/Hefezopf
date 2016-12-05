using System;
using Gsaelzbrot.Library;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Creates instances that implement special interface(s).
    /// </summary>
    public class HZDBModelFactory
        : IGsbModelFactory {
        /// <summary>
        /// Creates a schema.
        /// </summary>
        /// <returns>a new schema.</returns>
        public IGsbSchema CreateGsbSchema() {
            return new HZDBSchema();
        }

        /// <summary>
        /// Create a new table
        /// </summary>
        /// <returns>a new tables</returns>
        public GsbTable CreateGsbTable() {
            return new HZDBTable();
        }

        /// <summary>
        /// Create a new Column.
        /// </summary>
        /// <returns>A new Column.</returns>
        public GsbColumn CreateGsbColumn() {
            return new HZDBColumn();
        }

        /// <summary>
        /// Create a new DataTape.
        /// </summary>
        /// <returns>A new DataTape</returns>
        public GsbDataType CreateGsbDataType() {
            return new HZDBDataType();
        }

        /// <summary>
        /// Create a new index.
        /// </summary>
        /// <returns>A new index.</returns>
        public GsbIndex CreateGsbIndex() {
            return new HZDBIndex();
        }

        /// <summary>
        /// Create a new column for a index.
        /// </summary>
        /// <returns>a new column for a index.</returns>
        public GsbIndexedColumn CreateGsbIndexedColumn() {
            return new HZDBIndexedColumn();
        }

        /// <summary>
        /// Create a new view.
        /// </summary>
        /// <returns>a new view.</returns>
        public GsbView CreateGsbView() {
            return new HZDBView();
        }

        /// <summary>
        /// Create a new Stored Procedure.
        /// </summary>
        /// <returns>a new Stored Procedure.</returns>
        public GsbStoredProcedure CreateGsbStoredProcedure() {
            return new HZDBStoredProcedure();
        }

        /// <summary>
        /// Create a new funtion.
        /// </summary>
        /// <returns>a new funtion.</returns>
        public GsbFunction CreateGsbFunction() {
            return new HZDBFuntion();
        }

        /// <summary>
        /// Create a new parameter.
        /// </summary>
        /// <returns>a new parameter.</returns>
        public GsbParameter CreateGsbParameter() {
            return new HZDBParameter();
        }
    }
}