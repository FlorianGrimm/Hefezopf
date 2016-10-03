using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class FuncstructorConfigurationAttribute : Attribute
    {
        public readonly Type RegisterType;
        /// <summary>
        /// which type should be called must be a <see cref="T:IFuncstructorConfiguration"/>.
        /// </summary>
        /// <param name="registerType"></param>
        public FuncstructorConfigurationAttribute(Type registerType)
        {
            RegisterType = registerType;
        }
    }
}
