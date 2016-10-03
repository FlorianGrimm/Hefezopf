using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    [AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Method)]
    public class FuncstructorRegisterAttribute : Attribute
    {
        public readonly object Key;
        public FuncstructorRegisterAttribute()
        {
        }
        public FuncstructorRegisterAttribute(object key)
        {
            this.Key = key;
        }
    }
}
