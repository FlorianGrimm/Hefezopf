namespace Hefezopf.WindowsService.Shared {
    using System.Linq;
    //[RunInstaller(true)]
    public partial class HZProjectInstallerBase : System.Configuration.Install.Installer {
        public HZProjectInstallerBase(string name, string serviceUsername, string servicePassword) {
            //if (System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Break(); } else { System.Diagnostics.Debugger.Launch(); }
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            if (string.IsNullOrEmpty(serviceUsername) || string.Equals("LocalService", serviceUsername, System.StringComparison.OrdinalIgnoreCase)) {
                this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
                this.serviceProcessInstaller1.Username = null;
                this.serviceProcessInstaller1.Password = null;
            } else if (string.Equals("NetworkService", serviceUsername, System.StringComparison.OrdinalIgnoreCase)) {
                this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.NetworkService;
                this.serviceProcessInstaller1.Username = null;
                this.serviceProcessInstaller1.Password = null;
            } else {
                this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
                this.serviceProcessInstaller1.Username = serviceUsername;
                this.serviceProcessInstaller1.Password = servicePassword;
                using (var lsa = new LsaWrapper()) {
                    var rights = lsa.EnumerateAccountPrivileges(serviceUsername);
                    if (!rights.Any(_ => _ == Rights.SeServiceLogonRight)) {
                        lsa.AddPrivilege(serviceUsername, Rights.SeServiceLogonRight);
                    }
                }
            }
            // 
            this.serviceInstaller1.DisplayName = name;
            this.serviceInstaller1.ServiceName = name;
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
