namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represent userdefined type.
    /// </summary>
    public class HZDBUserDefinedType : HZDBSchemaOwned {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBUserDefinedType"/> class.
        /// </summary>
        public HZDBUserDefinedType() {
        }

        public System.Data.SqlDbType Type { get; set; }

        public int Size { get; set; }
    }
}