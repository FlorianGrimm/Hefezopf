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
        /// Initializes a new instance of the <see cref="FuncstructorConfigurationAttribute"/> class.
        /// which type should be called must be a <see cref="T:IFuncstructorConfiguration"/>.
        /// </summary>
        /// <param name="registerType">the type registers the factories.</param>
        public FuncstructorConfigurationAttribute(Type registerType)
        {
            this.RegisterType = registerType;
        }
    }
}
