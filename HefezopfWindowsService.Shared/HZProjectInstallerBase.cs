namespace HefezopfWindowsService.Shared {
    //[RunInstaller(true)]
    public partial class HZProjectInstallerBase : System.Configuration.Install.Installer {
        private readonly string _Name;
        public HZProjectInstallerBase(string name) {
            this._Name = name;
            //if (System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Break(); } else { System.Diagnostics.Debugger.Launch(); }
            this.InitializeComponent();
        }
        private void InitializeComponent() {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            this.serviceInstaller1.DisplayName = this._Name;
            this.serviceInstaller1.ServiceName = this._Name;
            // 
            this.Installers.AddRange(
                    new System.Configuration.Install.Installer[] {
                        this.serviceProcessInstaller1,
                        this.serviceInstaller1});
        }

        protected System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        protected System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}
