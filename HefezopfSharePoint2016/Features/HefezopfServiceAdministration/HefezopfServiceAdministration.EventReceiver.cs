// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.Administration.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;

#pragma warning disable SA1649 // File name must match first type name.

    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [System.Runtime.InteropServices.Guid("e2376b02-d357-4510-9f4b-ec0b613d16c7")]
    public class HefezopfServiceAdministrationEventReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Feature installed event.
        /// </summary>
        /// <param name="properties">The feature properties.</param>
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            try
            {
                // Ensure that the resource files in CONFIG\ADMINRESOURCES are copied to App_GlobalResources.
                // If you have Central Administration on another server, you will need to run
                // stsadm -o copyappbincontent or Install-SPApplicationContent on that server directly,
                // as the call below only runs on the server that the WSP is deployed on.
                SPWebService.AdministrationService.ApplyApplicationContentToLocalServer();
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("Service Applications", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
