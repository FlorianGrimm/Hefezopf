using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.Schema
{
    /// <summary>
    /// Basetype for object that have a schema.
    /// </summary>
    public class HZDBSchemaOwned
        : HZNamed
        , IHZDBOwned<HZDBSchema>
    {
        private HZDBSchema _Schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBSchemaOwned"/> class.
        /// </summary>
        public HZDBSchemaOwned()
        {
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        public HZDBSchema Schema
        {
            get
            {
                return this._Schema;
            }
        }

        /// <summary>
        /// Set the owned schema.
        /// </summary>
        /// <param name="owner">the owned schema.</param>
        public void SetOwner(HZDBSchema owner)
        {
            if (ReferenceEquals(this._Schema, owner)) { return; }
            var oldSchema = this._Schema;
            this._Schema = owner;
            this.SetSchema(oldSchema, owner);
        }

        protected virtual void SetSchema(HZDBSchema oldSchema, HZDBSchema newSchema)
        {
            //if (oldSchema != null) { oldSchema.Tables.Remove(); }
        }

        protected static void SetSchemaForThat<T>(HZDBSchema oldSchema, HZDBSchema newSchema, T that, Func<HZDBSchema, IList<T>> getList)
        {
            if (oldSchema != null)
            {
                var lst = getList(oldSchema);
                lst.Remove(that);
            }
            if (newSchema != null)
            {
                var lst = getList(newSchema);
                if (!lst.Any(_ => ReferenceEquals(_, that)))
                {
                    lst.Add(that);
                }
            }
        }
    }
}
