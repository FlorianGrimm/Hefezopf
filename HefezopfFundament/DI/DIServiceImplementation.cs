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
        private readonly IAssemblyWatcherService _AssemblyWatcherService;

        public DIServiceImplementation() {
            var assemblyWatcherService = new AssemblyWatcherService();
            this._AssemblyWatcherService = assemblyWatcherService;
            this._DependencyInjection = Funcstructor.CreateDefault(assemblyWatcherService);
        }

        public override IDependencyInjection GetGlobalInstance()
        {
            return this._DependencyInjection;
        }
    }
}
