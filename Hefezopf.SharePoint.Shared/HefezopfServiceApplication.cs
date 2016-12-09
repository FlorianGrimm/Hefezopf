// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Web;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;

    /// <summary>
    /// The Service Application.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true)]
    [IisWebServiceApplicationBackupBehavior]
    [System.Runtime.InteropServices.Guid("8c2ac481-3ec8-4b04-ab06-320648ecc16b")]
    public sealed class HefezopfServiceApplication : SPIisWebServiceApplication
    {
        #region Fields

        /// <summary>
        /// The service application database.
        /// </summary>
        [Persisted]
        private HefezopfDatabase database;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceApplication"/> class. Default constructor (required for SPPersistedObject serialization). Never call this directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HefezopfServiceApplication()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceApplication"/> class. Use this constructor when creating a new Service Application (e.g. from code in your Create page)
        /// </summary>
        /// <param name="name">The name of the service application.</param>
        /// <param name="service">The <see cref="HefezopfIisWebService" />.</param>
        /// <param name="applicationPool">The application pool.</param>
        public HefezopfServiceApplication(string name, HefezopfIisWebService service, SPIisWebServiceApplicationPool applicationPool)
            : base(name, service, applicationPool)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceApplication"/> class. Use this constructor when creating a new Service Application (e.g. from code in your Create page)
        /// </summary>
        /// <param name="name">The name of the service application.</param>
        /// <param name="service">The <see cref="HefezopfIisWebService" />.</param>
        /// <param name="customDatabase">A custom database to associate with this service application.</param>
        /// <param name="applicationPool">The application pool.</param>
        public HefezopfServiceApplication(string name, HefezopfIisWebService service, HefezopfDatabase customDatabase, SPIisWebServiceApplicationPool applicationPool)
            : base(name, service, applicationPool)
        {
            if (customDatabase == null)
            {
                throw new ArgumentNullException("customDatabase");
            }

            this.database = customDatabase;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TypeName. This string will display in the Type column on the Manage Service Applications screen. You can localize this value. If you don't override this,
        /// the default string in the Type column will be the name of this type from GetType().
        /// </summary>
        public override string TypeName
        {
            get { return SPUtility.GetLocalizedString("$Resources:ServiceApplicationName", "Hefezopf.Service.ServiceResources", (uint)System.Threading.Thread.CurrentThread.CurrentCulture.LCID); }
        }

        /// <summary>
        /// Gets the link to the Management page for this service application. Use this page to provide a UI for changing and configuring your application-specific settings.
        /// </summary>
        public override SPAdministrationLink ManageLink
        {
            get
            {
                return new SPAdministrationLink(string.Format(CultureInfo.InvariantCulture, "/_admin/Hefezopf.Service/ManageApplication.aspx?{0}", SPHttpUtility.UrlKeyValueEncode("id", this.Id.ToString())));
            }
        }

        /// <summary>
        /// Gets the link to the Properties page for this service application. Use this page to enable the user to change basic settings such as the Application Pool.
        /// </summary>
        public override SPAdministrationLink PropertiesLink
        {
            get
            {
                return new SPAdministrationLink(string.Format(CultureInfo.InvariantCulture, "/_admin/Hefezopf.Service/Properties.aspx?{0}", SPHttpUtility.UrlKeyValueEncode("id", this.Id.ToString())));
            }
        }

        /// <summary>
        /// Gets the current version number of this service application. This number should match the number in the ServiceProxy's SupportedServiceApplication attribute.
        /// </summary>
        public override Version ApplicationVersion
        {
            get
            {
                return new Version("1.0.0.0");
            }
        }

        /// <summary>
        /// Gets the Class Id. This is used in the SupportedServiceApplication attribute.
        /// </summary>
        public override Guid ApplicationClassId
        {
            get
            {
                return new Guid("7691d427-3a83-4ce3-ad0a-15b3e143ed27");
            }
        }

        /// <summary>
        /// Gets or sets the custom database.
        /// </summary>
        public HefezopfDatabase Database
        {
            get
            {
                return this.database;
            }

            set
            {
                this.database = value;
            }
        }

        /// <summary>
        /// Gets the Virtual Path of the service. The path to your services SVC file.
        /// </summary>
        /// <remarks>
        /// Service applications only support one SVC file. To support multiple svc files in a single service application,
        /// use a placeholder string here, and replace it in your Service Application Client at runtime after the load-balanced url has been acquired.
        /// </remarks>
        protected override string VirtualPath
        {
            get { return "ReplaceableEndPointName.svc"; }
        }

        /// <summary>
        /// Gets the install path to the subfolder of the WebServices folder.
        /// </summary>
        protected override string InstallPath
        {
            get { return SPUtility.GetVersionedGenericSetupPath(@"WebServices\Hefezopf.Service", 15); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is responsible for provisioning the Service Application. This override is where you can install any timer jobs or other application components.
        /// </summary>
        public override void Provision()
        {
            // First change the status of the object, this is not done in the base class implementations of Provision.
            if (this.Status != SPObjectStatus.Provisioning)
            {
                this.Status = SPObjectStatus.Provisioning;
                this.Update();
            }

            // Call the base implementation. The base class will update the object and set its status to Online.
            base.Provision();
        }

        /// <summary>
        /// Removes the Service Application. If using a custom persisted database, include logic to remove the DB as well here. Don't forget to call Delete afterwards.
        /// </summary>
        /// <param name="deleteData">Whether to delete data associated with this service application.</param>
        public override void Unprovision(bool deleteData)
        {
            // First mark the status, this is not done in the base class implementations of Unprovision
            if (this.Status != SPObjectStatus.Unprovisioning)
            {
                this.Status = SPObjectStatus.Unprovisioning;
                this.Update();
            }

            // Unprovision this parent object first. The base class will Update the object and set its status to Disabled.
            base.Unprovision(deleteData);

            // Unprovision child objects (databases, timer jobs, etc)
            if (deleteData)
            {
                if (this.database != null)
                {
                    this.database.Unprovision();
                }
            }
        }

        /// <summary>
        /// Deletes the Service Application from the Persisted Object Store. Use this to delete child objects such as custom
        /// databases or Timer Jobs.
        /// </summary>
        public override void Delete()
        {
            // Delete this parent object first, otherwise we can't delete dependent objects like a Database later.
            base.Delete();

            if (this.database != null)
            {
                this.database.Delete();
                this.database = null;
            }
        }

        /// <summary>
        /// This will be called during initial provisioning, and if the Application Pool is ever changed.
        /// </summary>
        /// <param name="processSecurityIdentifier">A security identifier.</param>
        protected override void OnProcessIdentityChanged(SecurityIdentifier processSecurityIdentifier)
        {
            base.OnProcessIdentityChanged(processSecurityIdentifier);

            if (this.database != null)
            {
                this.database.GrantApplicationPoolAccess(processSecurityIdentifier);
            }
        }

        #endregion
    }
}