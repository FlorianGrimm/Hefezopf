using System;
using System.Collections.Generic;
using Gsaelzbrot.Library;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represents a stored procedure
    /// </summary>
    public class HZDBStoredProcedure
        : HZDBSchemaOwned
        , GsbStoredProcedure {
        private readonly List<HZDBParameter> _Parameters;
        private readonly HZCastingList<GsbParameter, HZDBParameter> _GsbParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBStoredProcedure"/> class.
        /// </summary>
        public HZDBStoredProcedure() {
            this._Parameters = new List<HZDBParameter>();
            this._GsbParameters = new HZCastingList<GsbParameter, HZDBParameter>(this._Parameters, (gsb) => (HZDBParameter)gsb, (hz) => (GsbParameter)hz);
        }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public List<HZDBParameter> Parameters { get { return this._Parameters; } set { SetterListProperty(this._Parameters, value); } }

        IList<GsbParameter> GsbStoredProcedure.Parameters { get { return this._GsbParameters; } set { SetterListProperty(this._GsbParameters, value); } }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string TextHeader { get; set; }
    }
}