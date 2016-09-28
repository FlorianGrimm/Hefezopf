using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    public interface IFuncstructorConfiguration
    {
        void Register(IDependencyInjectionConfigurable funcstructor);
    }
}
