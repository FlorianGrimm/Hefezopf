// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.ComponentModel;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;

    /// <summary>
    /// The Service Proxy. This is registered once per farm.
    /// </summary>
    [System.Runtime.InteropServices.Guid("75dba667-9703-482c-824b-3f5bf2bcd103")]
    [SupportedServiceApplication("7691d427-3a83-4ce3-ad0a-15b3e143ed27", "1.0.0.0", typeof(HefezopfServiceApplicationProxy))]
    public sealed class HefezopfServiceProxy : SPIisWebServiceProxy, IServiceProxyAdministration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceProxy"/> class. Default constructor (required for SPPersistedObject serialization). Never call this directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HefezopfServiceProxy()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceProxy"/> class. Use this constructor to install the service proxy on servers in the farm.
        /// </summary>
        /// <param name="farm">The <see cref="Microsoft.SharePoint.Administration.SPFarm" /> to install the Service Proxy to.</param>
        internal HefezopfServiceProxy(SPFarm farm)
            : base(farm)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the service application proxy during the farm configuration wizard, and when you attempt to create a connection to a cross-farm service application.
        /// </summary>
        /// <param name="serviceApplicationProxyType">The <see cref="System.Type" /> of the service application proxy.</param>
        /// <param name="name">The name of the service application proxy.</param>
        /// <param name="serviceApplicationUri">The service application uri.</param>
        /// <param name="provisioningContext">The <see cref="Microsoft.SharePoint.Administration.SPServiceProvisioningContext" />.</param>
        /// <returns>An <see cref="Microsoft.SharePoint.Administration.SPServiceApplicationProxy" />.</returns>
        public SPServiceApplicationProxy CreateProxy(System.Type serviceApplicationProxyType, string name, System.Uri serviceApplicationUri, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplicationProxyType != typeof(HefezopfServiceApplicationProxy))
            {
                throw new NotSupportedException();
            }

            // First create the new proxy.
            HefezopfServiceApplicationProxy proxy = new HefezopfServiceApplicationProxy(name, this, serviceApplicationUri);

            // You must call Update() to get the object into the persisted store before you can provision.
            proxy.Update();

            // Provision (install) the proxy.
            proxy.Provision();

            return proxy;
        }

        /// <summary>
        /// Gets the service application proxy type description, for display in the Farm Configuration Wizard and in the Publish and Connect pages for cross-farm service application setup. You can localize these values.
        /// </summary>
        /// <param name="serviceApplicationProxyType">The <see cref="System.Type" /> of the service application.</param>
        /// <returns>An <see cref="Microsoft.SharePoint.Administration.SPPersistedTypeDescription" />.</returns>
        public SPPersistedTypeDescription GetProxyTypeDescription(System.Type serviceApplicationProxyType)
        {
            if (serviceApplicationProxyType != typeof(HefezopfServiceApplicationProxy))
            {
                throw new NotSupportedException();
            }

            return new SPPersistedTypeDescription(
                SPUtility.GetLocalizedString("$Resources:ServiceApplicationProxyName", "Hefezopf.Service.ServiceResources", (uint)System.Threading.Thread.CurrentThread.CurrentCulture.LCID),
                SPUtility.GetLocalizedString("$Resources:ServiceApplicationDescription", "Hefezopf.Service.ServiceResources", (uint)System.Threading.Thread.CurrentThread.CurrentCulture.LCID));
        }

        /// <summary>
        /// Gets the available proxy types.
        /// </summary>
        /// <returns>An array of <see cref="System.Type" />.</returns>
        public Type[] GetProxyTypes()
        {
            return new Type[] { typeof(HefezopfServiceApplicationProxy) };
        }

        /// <summary>
        /// Gets an existing service proxy or creates it if it doesn't exist.
        /// </summary>
        /// <returns>An instance of this service proxy.</returns>
        internal static HefezopfServiceProxy GetOrCreateServiceProxy()
        {
            HefezopfServiceProxy serviceProxy = SPFarm.Local.ServiceProxies.GetValue<HefezopfServiceProxy>();
            if (serviceProxy == null)
            {
                serviceProxy = new HefezopfServiceProxy(SPFarm.Local);
                serviceProxy.Status = SPObjectStatus.Online;
                serviceProxy.Update();
            }

            return serviceProxy;
        }

        #endregion
    }
}
