using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class FuncstructorFactoryAttribute : Attribute
    {
        public readonly Type FactoryType;
        /// <summary>
        /// which type should be called must be a <see cref="T:IFuncstructorConfiguration"/>.
        /// </summary>
        /// <param name="factoryType"></param>
        public FuncstructorFactoryAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }
    }
}
