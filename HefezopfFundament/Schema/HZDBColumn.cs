namespace Hefezopf.Fundament.Schema {
    using Gsaelzbrot.Library;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent a column.
    /// </summary>
    public class HZDBColumn
        : HZNamed
        , GsbColumn {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBColumn"/> class.
        /// </summary>
        public HZDBColumn() {
        }

        /// <summary>
        /// Gets or sets the DataType.
        /// </summary>
        public HZDBDataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the collation.
        /// </summary>
        public string Collation { get; set; }

        GsbDataType GsbColumn.DataType {
            get {
                return this.DataType;
            }

            set {
                if (value == null) {
                    this.DataType = null;
                } else {
                    this.DataType = (value as HZDBDataType) ?? new HZDBDataType(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column has incremental identity.
        /// </summary>
        public bool Identity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is NULL-able.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Gets or sets the default expression.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Gets or sets the computed Text
        /// </summary>
        public string ComputedText { get; set; }

        /// <summary>
        /// Sets the DataType-
        /// </summary>
        /// <param name="sqlDbType">the type as SqlDbType.</param>
        /// <returns>this</returns>
        public HZDBColumn SetDbType(System.Data.SqlDbType sqlDbType) {
            this.DataType = HZDBDataType.From(sqlDbType);
            return this;
        }
    }
}
