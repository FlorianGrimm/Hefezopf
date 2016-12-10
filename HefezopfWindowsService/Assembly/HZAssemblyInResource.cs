namespace Hefezopf.WindowsService.Assembly {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HZAssemblyInResource : global::Hefezopf.AssemblyInResource.Shared.HZAssemblyInResource {
#if DEBUG
        private const string SubPath = "Debug";
#else
        private const string SubPath = "Release";
#endif
        public HZAssemblyInResource() : base(null, SubPath) {

        }
    }
}
