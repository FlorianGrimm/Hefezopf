﻿namespace Hefezopf.Sharepoint.WindowsService {
    using Hefezopf.WindowsService.Shared;
    using System;
    using System.ComponentModel;
    using System.ServiceProcess;

    public class HZServiceImplementation : IHZService, IDisposable {
        private bool _IsDisposed;
        private Hefezopf.Sharepoint.WindowsService.Assembly.HZAssemblyInResource _AssemblyInResource;
        private string _ServiceName;

        public HZServiceImplementation() {
        }
        public void SetOwner(ServiceBase owner) {
            this._ServiceName = owner.ServiceName;
        }
        public void Start(object args) {
            this._AssemblyInResource = new Hefezopf.Sharepoint.WindowsService.Assembly.HZAssemblyInResource();
            var binFolderPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                , "bin");
            this._AssemblyInResource.ExtractTo(binFolderPath);
            var assembly = System.Reflection.Assembly.Load("Hefezopf.Contracts, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = 2d8ec664ab7d64f1");
        }

        public void Stop() {
            using (var d = this._AssemblyInResource) {
                this._AssemblyInResource = null;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._IsDisposed) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                using (var d = this._AssemblyInResource) {
                    this._AssemblyInResource = null;
                }
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
    public partial class ProjectInstaller : global::Hefezopf.WindowsService.Shared.HZProjectInstallerBase {
        public ProjectInstaller() : base(
            global::Hefezopf.WindowsService.Shared.HZBootingService.ServiceNameForInstaller ?? Consts.DefaultServiceName,
            global::Hefezopf.WindowsService.Shared.HZBootingService.ServiceUsernameForInstaller,
            global::Hefezopf.WindowsService.Shared.HZBootingService.ServicePasswordForInstaller
            ) {
        }
    }
}
