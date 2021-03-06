﻿// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.WebControls;
    using Hefezopf.Service;

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
            if (!this.Page.IsPostBack)
            {
                // Populate the controls with default values
                HefezopfIisWebService service = SPFarm.Local.Services.GetValue<HefezopfIisWebService>();
                HefezopfServiceProxy proxy = SPFarm.Local.ServiceProxies.GetValue<HefezopfServiceProxy>();

                if (service != null)
                {
                    this.literalServiceStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceStatusLabelInstalled", CultureInfo.CurrentCulture).ToString();
                    this.imageServiceStatus.ImageUrl = "/_admin/Hefezopf.Service/ServiceInstalled.gif";
                    this.buttonInstallService.Visible = false;
                    this.buttonRemoveService.Visible = true;

                    this.serviceInstanceStatusSection.Visible = true;
                    this.serviceApplicationStatusSection.Visible = true;
                }
                else
                {
                    this.literalServiceStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceStatusLabelNotInstalled", CultureInfo.CurrentCulture).ToString();
                    this.imageServiceStatus.ImageUrl = "/_admin/Hefezopf.Service/ServiceNotInstalled.gif";
                    this.buttonInstallService.Visible = true;
                    this.buttonRemoveService.Visible = false;
                    this.buttonInstallServiceInstances.Visible = false;

                    this.serviceInstanceStatusSection.Visible = false;
                    this.serviceApplicationStatusSection.Visible = false;
                }

                if (proxy != null)
                {
                    this.imageServiceProxyStatus.ImageUrl = "/_admin/Hefezopf.Service/ServiceInstalled.gif";
                    this.literalServiceProxyStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceStatusLabelInstalled", CultureInfo.CurrentCulture).ToString();
                }
                else
                {
                    this.imageServiceProxyStatus.ImageUrl = "/_admin/Hefezopf.Service/ServiceNotInstalled.gif";
                    this.literalServiceProxyStatus.Text = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceStatusLabelNotInstalled", CultureInfo.CurrentCulture).ToString();
                }

                // Check if the service instances have been provisioned, and show their status
                List<ServiceInstanceStatus> serviceInstances = new List<ServiceInstanceStatus>();

                foreach (SPServer server in SPFarm.Local.Servers)
                {
#if SP2016
                    var isValidRole = (server.Role == SPServerRole.Application || server.Role == SPServerRole.SingleServerFarm || server.Role == SPServerRole.WebFrontEnd);
#else
                    var isValidRole = (server.Role == SPServerRole.Application || server.Role == SPServerRole.SingleServer || server.Role == SPServerRole.WebFrontEnd);
#endif
                    if (isValidRole)
                    {
                        ServiceInstanceStatus serviceInstance = new ServiceInstanceStatus();
                        serviceInstance.ServerName = server.Name;
                        serviceInstance.ServerId = HttpUtility.UrlEncode(server.Id.ToString());
                        serviceInstances.Add(serviceInstance);

                        HefezopfServiceInstance instance = server.ServiceInstances.GetValue<HefezopfServiceInstance>();
                        if (instance == null)
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstanceStatusNotInstalled", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = false;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf.Service/HLTHFAIL.PNG";
                        }
                        else if (instance.Status == SPObjectStatus.Online)
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstanceStatusStarted", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = true;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf.Service/HLTHSUCC.PNG";
                        }
                        else
                        {
                            serviceInstance.ServerStatus = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstanceStatusStopped", CultureInfo.CurrentCulture).ToString();
                            serviceInstance.IsInstalled = true;
                            serviceInstance.ServerStatusImage = "/_admin/Hefezopf.Service/HLTHERR.PNG";
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
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstallOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstallOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                // Install the service
                HefezopfIisWebService service = HefezopfIisWebService.GetOrCreateService();

                // Install the service instances but do not start (provision) them (let the admin do this on the services on server screen).
                HefezopfServiceInstance.CreateServiceInstances(service);

                // Install the service proxy
                HefezopfServiceProxy.GetOrCreateServiceProxy();

                operation.End("/_admin/Hefezopf.Service/ManageService.aspx");
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
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceRemoveOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceRemoveOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                HefezopfIisWebService.RemoveService();

                operation.End("/_admin/Hefezopf.Service/ManageService.aspx");
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
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstallServiceInstancesOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ManageServiceInstallServiceInstancesOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                // Get the service
                HefezopfIisWebService service = HefezopfIisWebService.GetOrCreateService();

                // Create the service instances
                HefezopfServiceInstance.CreateServiceInstances(service);

                operation.End("/_admin/Hefezopf.Service/ManageService.aspx");
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
