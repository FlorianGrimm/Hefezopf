// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.ComponentModel;
    using Microsoft.SharePoint.Administration;

    /// <summary>
    /// The service instance. Appears on the Services on Server screen in SharePoint Central Administration. There can be
    /// one service instance per server on the farm. Administrators can stop/start the service on individual servers. Each
    /// server that the service is started on will participate in the automatic load-balancing in the service application proxy.
    /// </summary>
    [System.Runtime.InteropServices.Guid("ba7b0518-9024-41b2-8132-26deafc28b5d")]
    public sealed class HefezopfServiceInstance : SPIisWebServiceInstance
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceInstance"/> class. Default constructor (required for SPPersistedObject serialization). Never call this directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HefezopfServiceInstance()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceInstance"/> class. Use this constructor to install the service instance on servers in the farm.
        /// </summary>
        /// <param name="server">The SPServer to install the instance to.</param>
        /// <param name="service">The service to associate the service instance with.</param>
        internal HefezopfServiceInstance(SPServer server, HefezopfIisWebService service)
            : base(server, service)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the management link. This property makes the Service Instance a clickable hyperlink on the Services on Server page.
        /// </summary>
        public override SPActionLink ManageLink
        {
            get
            {
                return new SPActionLink("/_admin/Hefezopf.Service/ManageService.aspx");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Installs the service instances on servers in the farm (does not start them).
        /// </summary>
        /// <param name="service">The service associated with these instances.</param>
        internal static void CreateServiceInstances(HefezopfIisWebService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            foreach (SPServer server in SPFarm.Local.Servers)
            {
#if SP2016
                if (server.Role == SPServerRole.Application || server.Role == SPServerRole.SingleServerFarm || server.Role == SPServerRole.WebFrontEnd)
                {
                    HefezopfServiceInstance instance = server.ServiceInstances.GetValue<HefezopfServiceInstance>();
                    if (instance == null)
                    {
                        instance = new HefezopfServiceInstance(server, service);
                        instance.Update();
                    }
                }
#elif SP2013
                if (server.Role == SPServerRole.Application || server.Role == SPServerRole.SingleServer || server.Role == SPServerRole.WebFrontEnd)
                {
                    HefezopfServiceInstance instance = server.ServiceInstances.GetValue<HefezopfServiceInstance>();
                    if (instance == null)
                    {
                        instance = new HefezopfServiceInstance(server, service);
                        instance.Update();
                    }
                }
#else
#error SP2016 || SP2013 needed
#endif
            }
        }

        #endregion
    }
}