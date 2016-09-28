using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.DI
{
    public interface IAssemblyLoaderService
    {
        void WireTo(IDependencyInjectionConfigurable funcstructor);
    }
}
