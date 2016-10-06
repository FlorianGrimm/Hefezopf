// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.PowerShell
{
    using System;
    using System.Management.Automation;
    using Hefezopf.Service;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.PowerShell;

    /// <summary>
    /// Creates a new Service Application Proxy.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "HefezopfServiceApplicationProxy", SupportsShouldProcess = true)]
    [SPCmdlet(RequireLocalFarmExist = true, RequireUserFarmAdmin = true)]
    [System.Runtime.InteropServices.Guid("321b8ff3-16a4-4895-a3ac-cbe3021a863f")]
    public class NewHefezopfServiceApplicationProxy : SPCmdlet
    {
        /// <summary>
        /// The service application.
        /// </summary>
        private SPServiceApplicationPipeBind application;

        /// <summary>
        /// The name of the proxy.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the service application to associate this proxy with.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Application", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public SPServiceApplicationPipeBind ServiceApplication
        {
            get
            {
                return this.application;
            }

            set
            {
                this.application = value;
            }
        }

        /// <summary>
        /// Gets or sets the URI to a published service application.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "URI")]
        [ValidateNotNullOrEmpty]
        public string Uri
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets whether the proxy is added to the default proxy group for the farm.
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter DefaultProxyGroup
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the name of the proxy
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Application")]
        [Parameter(Mandatory = true, ParameterSetName = "Uri")]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// This method gets invoked when the command is called
        /// </summary>
        protected override void InternalProcessRecord()
        {
            Uri applicationUri = null;
            SPServiceApplication resolvedApplication = null;
            HefezopfServiceApplication castedApplication = null;

            if (this.ServiceApplication == null && string.IsNullOrEmpty(this.Uri))
            {
                this.ThrowTerminatingError(new InvalidOperationException("No service application or Uri was provided."), ErrorCategory.InvalidOperation, this);
            }

            if (this.ServiceApplication != null)
            {
                resolvedApplication = this.ServiceApplication.Read();

                if (resolvedApplication == null)
                {
                    this.ThrowTerminatingError(new InvalidOperationException("Service application not found."), ErrorCategory.InvalidOperation, this);
                }

                castedApplication = resolvedApplication as HefezopfServiceApplication;

                if (castedApplication == null)
                {
                    this.ThrowTerminatingError(new InvalidOperationException("Service application was not of the correct type."), ErrorCategory.InvalidOperation, this);
                }

                applicationUri = castedApplication.Uri;

                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = castedApplication.Name + " Proxy";
                }
            }
            else
            {
                applicationUri = new Uri(this.Uri);
            }

            if (this.ShouldProcess(this.Name))
            {
                // Ensure the service exists
                HefezopfIisWebService.GetOrCreateService();

                // Ensure the proxy exists
                HefezopfServiceProxy serviceProxy = HefezopfServiceProxy.GetOrCreateServiceProxy();

                // Create the service application proxy
                HefezopfServiceApplicationProxy proxy = new HefezopfServiceApplicationProxy(this.Name, serviceProxy, applicationUri);
                proxy.Update();
                proxy.Provision();

                if (this.DefaultProxyGroup.ToBool())
                {
                    SPServiceApplicationProxyGroup group = SPServiceApplicationProxyGroup.Default;
                    group.Add(proxy);
                    group.Update();
                }

                this.WriteObject(proxy);
            }
        }

        /// <summary>
        /// Validate the arguments.
        /// </summary>
        protected override void InternalValidate()
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
        }
    }
}
