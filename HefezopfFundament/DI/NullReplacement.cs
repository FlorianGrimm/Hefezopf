using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    //http://csharpindepth.com/articles/general/singleton.aspx

    /// <summary>
    /// a singleton for another NULL
    /// </summary>
    public sealed class NullReplacement
    {
        private static readonly NullReplacement _NULL = new NullReplacement();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NullReplacement() { }

        private NullReplacement() { }
        public static object NULL
        {
            get
            {
                return _NULL;
            }
        }

        /// <summary>
        /// Determines whether obj is also this.
        /// </summary>
        /// <param name="obj">to test</param>
        /// <returns>true if obj is the same as this (NULL)</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// returns 0
        /// </summary>
        /// <returns>returns alwasy 0.</returns>
        public override int GetHashCode()
        {
            // must be null
            return 0;
        }
        public override string ToString()
        {
            return "NULL";
        }
    }
}
