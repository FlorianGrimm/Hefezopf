// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.PowerShell
{
    using System;
    using System.Management.Automation;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.PowerShell;

    /// <summary>
    /// Remove the service from the farm.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "HefezopfServiceApplication", SupportsShouldProcess = true)]
    [SPCmdlet(RequireLocalFarmExist = true, RequireUserFarmAdmin = true)]
    [System.Runtime.InteropServices.Guid("4858ed11-d1e5-4187-b944-b880ff7dfdbb")]
    public class RemoveHefezopfService : SPCmdlet
    {
        /// <summary>
        /// Gets or sets whether to delete any data associated with any service applications.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        [ValidateNotNullOrEmpty]
        public SwitchParameter DeleteData
        {
            get;

            set;
        }

        /// <summary>
        /// This method gets invoked when the command is called
        /// </summary>
        protected override void InternalProcessRecord()
        {
            // Validate a farm exists
            SPFarm farm = SPFarm.Local;
            if (farm == null)
            {
                this.ThrowTerminatingError(new InvalidOperationException("SharePoint server farm not found."), ErrorCategory.ResourceUnavailable, this);
            }

            SPServer server = SPServer.Local;
            if (server == null)
            {
                this.ThrowTerminatingError(new InvalidOperationException("SharePoint local server not found."), ErrorCategory.ResourceUnavailable, this);
            }

            if (this.ShouldProcess(SPFarm.Local.Name))
            {
                // Remove the service
                HefezopfIisWebService.RemoveService();
            }
        }
    }
}
