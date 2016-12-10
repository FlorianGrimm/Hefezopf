//-----------------------------------------------------------------------
// <copyright file="ManageService.aspx.cs" company="">
// Copyright © 
// </copyright>
//-----------------------------------------------------------------------

namespace Hefezopf.SharePoint.Application.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.WebControls;
    using Service;

    /// <summary>
    /// This page is responsible for provisioning (installing) the Service, ServiceProxy, and ServiceInstances.
    /// </summary>
    public partial class ManageServicePage : BaseAdminPage
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Page Events

        /// <summary>
        /// Page_Load event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Populate the controls with default values
                HefezopfService service = SPFarm.Local.Services.GetValue<HefezopfService>();
                HefezopfServiceProxy proxy = SPFarm.Local.ServiceProxies.GetValue<HefezopfServiceProxy>();

                if (service != null)
                {
                    this.literalServiceStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceStatusLabelInstalled", CultureInfo.CurrentCulture).ToString();
                    this.imageServiceStatus.ImageUrl = "/_admin/Hefezopf/ServiceInstalled.gif";
                    this.buttonInstallService.Visible = false;
                    this.buttonRemoveService.Visible = true;

                    this.serviceInstanceStatusSection.Visible = true;
                    this.serviceApplicationStatusSection.Visible = true;
                }
                else
                {
                    this.literalServiceStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceStatusLabelNotInstalled", CultureInfo.CurrentCulture).ToString();
                    this.imageServiceStatus.ImageUrl = "/_admin/Hefezopf/ServiceNotInstalled.gif";
                    this.buttonInstallService.Visible = true;
                    this.buttonRemoveService.Visible = false;
                    this.buttonInstallServiceInstances.Visible = false;

                    this.serviceInstanceStatusSection.Visible = false;
                    this.serviceApplicationStatusSection.Visible = false;
                }

                if (proxy != null)
                {
                    this.imageServiceProxyStatus.ImageUrl = "/_admin/Hefezopf/ServiceInstalled.gif";
                    this.literalServiceProxyStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceStatusLabelInstalled", CultureInfo.CurrentCulture).ToString();
                }
                else
                {
                    this.imageServiceProxyStatus.ImageUrl = "/_admin/Hefezopf/ServiceNotInstalled.gif";
                    this.literalServiceProxyStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceStatusLabelNotInstalled", CultureInfo.CurrentCulture).ToString();
                }

                // Check if the service instances have been provisioned, and show their status
                List<ServiceInstanceStatus> serviceInstances = new List<ServiceInstanceStatus>();

                foreach (SPServer server in SPFarm.Local.Servers)
                {
                    if (Shared.SharePointDifferences.ServerHasRoleForServiceInstance(server))
                    {
                        ServiceInstanceStatus serviceInstance = new ServiceInstanceStatus();
                        serviceInstance.ServerName = server.Name;
                        serviceInstance.ServerId = HttpUtility.UrlEncode(server.Id.ToString());
                        serviceInstances.Add(serviceInstance);

                        HefezopfServiceInstance instance = server.ServiceInstances.GetValue<HefezopfServiceInstance>();
                        if (instance == null)
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstanceStatusNotInstalled", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = false;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf/HLTHFAIL.PNG";
                        }
                        else if (instance.Status == SPObjectStatus.Online)
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstanceStatusStarted", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = true;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf/HLTHSUCC.PNG";
                        }
                        else
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstanceStatusStopped", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = true;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf/HLTHERR.PNG";
                        }
                    }
                }

                this.gridViewInstanceStatus.DataSource = serviceInstances;
                this.gridViewInstanceStatus.DataBind();

                if (service != null)
                {
                    this.gridViewApplicationStatus.DataSource = service.Applications;
                    this.gridViewApplicationStatus.DataBind();
                }
            }
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ButtonInstallService_Click(object sender, EventArgs e)
        {
            using (SPLongOperation operation = new SPLongOperation(this))
            {
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstallOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstallOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                // Install the service
                HefezopfService service = HefezopfService.GetOrCreateService();

                // Install the service instances but do not start (provision) them (let the admin do this on the services on server screen).
                HefezopfServiceInstance.CreateServiceInstances(service);

                // Install the service proxy
                HefezopfServiceProxy.GetOrCreateServiceProxy();

                operation.End("/_admin/Hefezopf/ManageService.aspx");
            }
        }

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ButtonRemoveService_Click(object sender, EventArgs e)
        {
            using (SPLongOperation operation = new SPLongOperation(this))
            {
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceRemoveOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceRemoveOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                HefezopfService.RemoveService();

                operation.End("/_admin/Hefezopf/ManageService.aspx");
            }
        }

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ButtonInstallServiceInstances_Click(object sender, EventArgs e)
        {
            using (SPLongOperation operation = new SPLongOperation(this))
            {
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstallServiceInstancesOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ManageServiceInstallServiceInstancesOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                // Get the service
                HefezopfService service = HefezopfService.GetOrCreateService();

                // Create the service instances
                HefezopfServiceInstance.CreateServiceInstances(service);

                operation.End("/_admin/Hefezopf/ManageService.aspx");
            }
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// Class used for data binding to the Service Instance grid view
        /// </summary>
        protected class ServiceInstanceStatus
        {
            /// <summary>
            /// Gets or sets the name of a server in the farm.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ASP.NET databinding.")]
            public string ServerName { get; set; }

            /// <summary>
            /// Gets or sets the status of the service instance on that server in the farm.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ASP.NET databinding.")]
            public string ServerStatus { get; set; }

            /// <summary>
            /// Gets or sets the Id of the Server
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ASP.NET databinding.")]
            public string ServerId { get; set; }

            /// <summary>
            /// Gets or sets a url to an image representing the status of the service instance on the server in the farm.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ASP.NET databinding.")]
            public string ServerStatusImage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the service instance is provisioned (installed) on a server in the farm.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ASP.NET databinding.")]
            public bool IsInstalled { get; set; }
        }

        #endregion
    }
}
