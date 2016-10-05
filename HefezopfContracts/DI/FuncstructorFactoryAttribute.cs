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
        /// Initializes a new instance of the <see cref="FuncstructorFactoryAttribute"/> class.
        /// which type should be called must be a <see cref="T:IFuncstructorConfiguration"/>.
        /// </summary>
        /// <param name="factoryType">the type of the factory</param>
        public FuncstructorFactoryAttribute(Type factoryType)
        {
            this.FactoryType = factoryType;
        }
    }
}
