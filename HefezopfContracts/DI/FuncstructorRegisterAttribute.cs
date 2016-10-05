using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FuncstructorRegisterAttribute : Attribute
    {
        public readonly object Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncstructorRegisterAttribute"/> class.
        /// </summary>
        /// <param name="key">TODO</param>
        public FuncstructorRegisterAttribute(object key)
        {
            this.Key = key;
        }
    }
}
