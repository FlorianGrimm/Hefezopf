namespace HefezopfWindowsService {
    using Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HZServiceImplementation : IHZService, IDisposable {
        private bool _IsDisposed;

        public HZServiceImplementation() {
        }

        public void Start(object args) {
        }

        public void Stop() {
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._IsDisposed) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this._IsDisposed = true;
            }
        }

        // ~HZServiceImplementation() {Dispose(false);}
        public void Dispose() {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
    }
        
    public class HZService : HZServiceBaseWithArguments {

        public HZService() : this(null) {
        }

        public HZService(string name) : base(name, Consts.DefaultServiceName) {
            //if (System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Break(); } else { System.Diagnostics.Debugger.Launch(); }
        }

        /*
        protected override void Parse(IEnumerable<string> args) {
            base.Parse(args);
        }
        protected override object GetStartArguments() {
            return base.GetStartArguments();
        }
        */
        protected override IHZService CreateServiceImplementation() {
            return new HZServiceImplementation();
        }
    }

    [RunInstaller(true)]
    public partial class ProjectInstaller : global::HefezopfWindowsService.Shared.HZProjectInstallerBase {
        public ProjectInstaller() : base(global::HefezopfWindowsService.Shared.HZBootingService.ServiceNameForInstaller ?? Consts.DefaultServiceName) {
        }
    }
}
