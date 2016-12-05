using System;
using Gsaelzbrot.Library;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represent a parameter (stored procedure, function, ..)
    /// </summary>
    public class HZDBParameter
        : GsbParameter {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBParameter"/> class.
        /// </summary>
        public HZDBParameter() {
        }

        /// <summary>
        /// Gets or sets the name of this parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the datatype of this parameter.
        /// </summary>
        public GsbDataType DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is readonly (TVP).
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
}