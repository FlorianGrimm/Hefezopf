namespace Hefezopf.Fundament.DI
{
    using Brimborium.Funcstructors;
    using Hefezopf.Contracts;
    using Hefezopf.Contracts.DI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The dependency injection service.
    /// </summary>
    public class DIServiceImplementation : DIService
    {
        private readonly IFuncstructor _GlobalFuncstructor;
        private readonly IAssemblyWatcherService _AssemblyWatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIServiceImplementation"/> class.
        /// </summary>
        public DIServiceImplementation() {
            var assemblyWatcherService = new AssemblyWatcherService();
            this._AssemblyWatcherService = assemblyWatcherService;
            this._GlobalFuncstructor = Funcstructor.CreateDefault(assemblyWatcherService);
        }

        /// <summary>
        /// Get an old or new instance of funcstructor.
        /// </summary>
        /// <returns>a funcstructor.</returns>
        public override IFuncstructor GetGlobalFuncstructor()
        {
            return this._GlobalFuncstructor;
        }
    }
}
