using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsaelzbrot.Library;

namespace Hefezopf.Fundament.Schema
{
    /// <summary>
    /// A reference to a datatype.
    /// </summary>
    public class HZDBDataType
        : HZDBSchemaOwned
        , GsbDataType
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDataType"/> class.
        /// </summary>
        public HZDBDataType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBDataType"/> class.
        /// Copy constructor.
        /// </summary>
        /// <param name="value">The source - can be null</param>
        public HZDBDataType(GsbDataType value)
        {
            if (value == null)
            {
                // do not do anything.
            }
            else
            {
                this.MaximumLength = value.MaximumLength;
                this.NumericPrecision = value.NumericPrecision;
                this.NumericScale = value.NumericScale;
                this.GsbSqlDataType = value.GsbSqlDataType;
            }
        }

        /// <summary>
        /// Gets or sets the MaximumLength
        /// </summary>
        public int MaximumLength { get; set; }

        /// <summary>
        /// Gets or sets the NumericPrecision
        /// </summary>
        public int NumericPrecision { get; set; }

        /// <summary>
        /// Gets or sets the NumericScale
        /// </summary>
        public int NumericScale { get; set; }

        /// <summary>
        /// Gets or sets the SqlDataType
        /// </summary>
        public GsbSqlDataType GsbSqlDataType { get; set; }

        /// <summary>
        /// Create on base of a <see cref="SqlDbType"/>.
        /// </summary>
        /// <param name="sqlDbType">the db type</param>
        /// <param name="maximumLength">the maximum length</param>
        /// <returns>a new instance representing the type</returns>
        public static HZDBDataType From(SqlDbType sqlDbType, int? maximumLength = null)
        {
            GsbSqlDataType dt = GsbSqlDataType.None;
            var result = new HZDBDataType();
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    dt = GsbSqlDataType.BigInt;
                    break;
                case SqlDbType.Binary:
                    dt = GsbSqlDataType.Binary;
                    break;
                case SqlDbType.Bit:
                    dt = GsbSqlDataType.Bit;
                    break;
                case SqlDbType.Char:
                    dt = GsbSqlDataType.Char;
                    break;
                case SqlDbType.DateTime:
                    dt = GsbSqlDataType.DateTime;
                    break;
                case SqlDbType.Decimal:
                    dt = GsbSqlDataType.Decimal;
                    break;
                case SqlDbType.Float:
                    dt = GsbSqlDataType.Float;
                    break;
                case SqlDbType.Image:
                    dt = GsbSqlDataType.Image;
                    break;
                case SqlDbType.Int:
                    dt = GsbSqlDataType.Int;
                    break;
                case SqlDbType.Money:
                    dt = GsbSqlDataType.Money;
                    break;
                case SqlDbType.NChar:
                    dt = GsbSqlDataType.NChar;
                    break;
                case SqlDbType.NText:
                    dt = GsbSqlDataType.NText;
                    break;
                case SqlDbType.NVarChar:
                    {
                        var ml = maximumLength.GetValueOrDefault();
                        if (ml < 0 || ml == int.MaxValue)
                        {
                            dt = GsbSqlDataType.NVarCharMax;
                            maximumLength = -1;
                        }
                        else
                        {
                            dt = GsbSqlDataType.NVarChar;
                        }
                    }
                    break;
                case SqlDbType.Real:
                    dt = GsbSqlDataType.Real;
                    break;
                case SqlDbType.UniqueIdentifier:
                    dt = GsbSqlDataType.UniqueIdentifier;
                    break;
                case SqlDbType.SmallDateTime:
                    dt = GsbSqlDataType.SmallDateTime;
                    break;
                case SqlDbType.SmallInt:
                    dt = GsbSqlDataType.SmallInt;
                    break;
                case SqlDbType.SmallMoney:
                    dt = GsbSqlDataType.SmallMoney;
                    break;
                case SqlDbType.Text:
                    dt = GsbSqlDataType.Text;
                    break;
                case SqlDbType.Timestamp:
                    dt = GsbSqlDataType.Timestamp;
                    break;
                case SqlDbType.TinyInt:
                    dt = GsbSqlDataType.TinyInt;
                    break;
                case SqlDbType.VarBinary:
                    dt = GsbSqlDataType.VarBinary;
                    break;
                case SqlDbType.VarChar:
                    {
                        var ml = maximumLength.GetValueOrDefault();
                        if (ml < 0 || ml == int.MaxValue)
                        {
                            dt = GsbSqlDataType.VarCharMax;
                            maximumLength = -1;
                        }
                        else
                        {
                            dt = GsbSqlDataType.VarChar;
                        }
                    }
                    break;
                case SqlDbType.Variant:
                    dt = GsbSqlDataType.Variant;
                    break;
                case SqlDbType.Xml:
                    dt = GsbSqlDataType.Xml;
                    break;
                case SqlDbType.Udt:
                    // TODO: check
                    dt = GsbSqlDataType.UserDefinedType;
                    break;
                case SqlDbType.Structured:
                    dt = GsbSqlDataType.UserDefinedTableType;
                    break;
                case SqlDbType.Date:
                    dt = GsbSqlDataType.Date;
                    break;
                case SqlDbType.Time:
                    dt = GsbSqlDataType.Time;
                    break;
                case SqlDbType.DateTime2:
                    dt = GsbSqlDataType.DateTime2;
                    break;
                case SqlDbType.DateTimeOffset:
                    dt = GsbSqlDataType.DateTimeOffset;
                    break;
                default:
                    break;
            }
            if (dt != GsbSqlDataType.None)
            {
                result.GsbSqlDataType = dt;
                return result;
            }
            else
            {
                throw new NotImplementedException(sqlDbType.ToString());
            }
        }
    }
}
