using Hefezopf.Contracts;
using Hefezopf.Contracts.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Fundament.DI
{
    public class DIServiceImplementation : DIService
    {
        private readonly IDependencyInjection _DependencyInjection;
        private readonly IAssemblyLoaderService _AssemblyLoaderService;

        public DIServiceImplementation() {
            var assmblyLoaderService = new AssemblyWatcherService();
            this._AssemblyLoaderService = assmblyLoaderService;
            this._DependencyInjection = Funcstructor.CreateDefault(assmblyLoaderService);
        }

        public override IDependencyInjection GetGlobalInstance()
        {
            return this._DependencyInjection;
        }
    }
}
