namespace Hefezopf.Sharepoint.WindowsService.Assembly {

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
