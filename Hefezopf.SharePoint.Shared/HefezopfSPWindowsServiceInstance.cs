namespace Hefezopf.SharePoint.Shared {
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Security;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Hefezopf.Contracts.Services;
    using Service;

    [Guid("1C0FC019-5383-40DA-8F50-9BCCD373C8B8")]
    [SharePointPermission(System.Security.Permissions.SecurityAction.LinkDemand, ObjectModel = true)]
    public class HefezopfSPWindowsServiceInstance : SPWindowsServiceInstance {

        /// <summary>Contructor.</summary>
        public HefezopfSPWindowsServiceInstance() { }

        /// <summary>Contructor.</summary>
        public HefezopfSPWindowsServiceInstance(string name, SPServer server, HefezopfSPWindowsService service) : base(name, server, service) { }

        /// <summary>TODO.</summary>
        public override void Provision() {
            //TODO.Later("check if local admin");
            if (SPServer.Local.Id == this.Server.Id) {
                this.ProvisionInternal();
            } else {
                var jobDef = new HefezopfSPWindowsServiceInstanceDeploymentJobDefinition(
                    this,
                    (HefezopfSPWindowsService)this.Service,
                    this.Server,
                    SPJobLockType.None,
                    HefezopfSPWindowsServiceInstanceDeploymentJobDefinition.ModeProvision
                    );
                jobDef.Update();
            }
        }

        /// <summary>TODO.</summary>
        internal void ProvisionInternal() {
            // TODO.Later("GetVersionedGenericSetupPath,16??");
            var spr_bin = Microsoft.SharePoint.Utilities.SPUtility.GetVersionedGenericSetupPath("bin", 15);
            var spr_bin_Hefezopf = System.IO.Path.Combine(spr_bin, "Hefezopf");
            var binPath = System.IO.Path.Combine(spr_bin_Hefezopf, "Hefezopf.Sharepoint.WindowsService.bin");
            var tempPath = System.IO.Path.Combine(spr_bin_Hefezopf, "Hefezopf.Sharepoint.WindowsService.temp.exe");
            var exePath = System.IO.Path.Combine(spr_bin_Hefezopf, "Hefezopf.Sharepoint.WindowsService.exe");
            //
            bool updateService = false;
            bool installService = false;
            if (!System.IO.File.Exists(binPath)) {
                HZLogger.Error(Consts.CategoryConfiguration, $"File not found {binPath}");
            }
            try {
                var exeExists = System.IO.File.Exists(exePath);
                if (exeExists) {
                    var dtBin = System.IO.File.GetLastWriteTimeUtc(binPath);
                    var dtExe = System.IO.File.GetLastWriteTimeUtc(exePath);
                    if (dtBin > dtExe) {
                        updateService = true;
                    }
                } else {
                    installService = true;
                }

            } catch (Exception exception) {
                HZLogger.CatchedError(Consts.CategoryConfiguration, exception);
                throw;
            }
            // copy
            // binPath
            // tempPath
            for (int watchDog = 1; watchDog < 10; watchDog++) {
                try {
                    System.IO.File.Copy(binPath, tempPath, true);
                    break;
                } catch (Exception exceptionCopy) {
                    HZLogger.Error(Consts.CategoryCommon, "Copy SPWinSvc Try {0}:{1} throws {2}", watchDog, binPath, exceptionCopy);
                    System.Threading.Thread.Sleep(500);
                }
            }
            var serviceId = this.Service.Id;
            string name = $"HefezopfSP-{serviceId}";

#warning Write configuration to ProgramData ...

            var spWindowsService = ((HefezopfSPWindowsService)this.Service);
            Guid serviceApplicationId = spWindowsService.ServiceApplicationId;
            HefezopfServiceApplication baseServiceApplication = spWindowsService.GetServiceApplication();

            if (installService) {
                HZLogger.Info(Consts.CategoryConfiguration, "Installing SPWinSvc:{0}", name);
                var psi = new System.Diagnostics.ProcessStartInfo(tempPath, $"/silent /install /name \"{name}\" /location \"{exePath}\"");
                var processUninstall = System.Diagnostics.Process.Start(psi);
                processUninstall.WaitForExit(3000);
            } else if (updateService) {
                HZLogger.Info(Consts.CategoryConfiguration, "Updating SPWinSvc:{0}", name);
                var psi = new System.Diagnostics.ProcessStartInfo(tempPath, $"/silent /update /name \"{name}\" /location \"{exePath}\"");
                var processUninstall = System.Diagnostics.Process.Start(psi);
                processUninstall.WaitForExit(3000);
            }
            try {
                base.Provision(true);
            } catch (Exception exc) {
                HZLogger.CatchedError(Consts.CategoryConfiguration, exc);
                throw;
            }
            //base.Start();
        }

        /// <summary>TODO.</summary>
        public override void Unprovision() {
            if (SPServer.Local.Id == this.Server.Id) {
                this.UnprovisionInternal();
            } else {
                var jobDef = new HefezopfSPWindowsServiceInstanceDeploymentJobDefinition(
                    this,
                    (HefezopfSPWindowsService)this.Service,
                    this.Server,
                    SPJobLockType.None,
                    HefezopfSPWindowsServiceInstanceDeploymentJobDefinition.ModeUnprovision);
                jobDef.Update();
            }
        }

        /// <summary>TODO.</summary>
        internal void UnprovisionInternal() {
            base.Stop();
            var spr_bin = Microsoft.SharePoint.Utilities.SPUtility.GetVersionedGenericSetupPath("bin", 15);
            var spr_bin_Hefezopf = System.IO.Path.Combine(spr_bin, "Hefezopf");
            //var exePath = System.IO.Path.Combine(spr_bin_Hefezopf, this.Service.Name + ".exe");
            var serviceId = this.Service.Id;
            string name = $"HefezopfSP-{serviceId}";
            var exePath = System.IO.Path.Combine(spr_bin_Hefezopf, "Hefezopf.Sharepoint.WindowsService.exe");
            if (System.IO.File.Exists(exePath)) {
                var psi = new System.Diagnostics.ProcessStartInfo(exePath, $"/silent /uninstall /name \"{name}\"");
                var processUninstall = System.Diagnostics.Process.Start(psi);
                processUninstall.WaitForExit(3000);
            }
#warning check if last...
            //try
            //{
            //    System.IO.File.Delete(exePath);
            //}
            //catch (Exception exception)
            //{
            //    HZLogger.Warning(Consts.CategoryConfiguration, "Cannot delete {0}: {1}", exePath, exception );
            //}
        }

        /// <summary>Gets the typename.</summary>
        public override string TypeName => "Hefezopf SP Windows Service Instance";

        /// <summary>Gets the displayname..</summary>
        public override string DisplayName => "Hefezopf SP Windows Service Instance";

        /// <summary>Gets a description.</summary>
        public override string Description => string.Format("Hefezopf SP Windows Service Instance {0}", (this.Server == null) ? "NULL" : this.Server.Name);
    }
    //
}
