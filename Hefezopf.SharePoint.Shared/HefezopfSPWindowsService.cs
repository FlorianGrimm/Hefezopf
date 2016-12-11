namespace Hefezopf.SharePoint.Shared {
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Security;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    [Guid("2B4B68B4-05DC-4713-9466-20442B1209F1")]
    [SharePointPermission(System.Security.Permissions.SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class HefezopfSPWindowsService : SPWindowsService {
        internal const string HostNameFormat = "HefezopfSPHost-{0}";
        internal const string HostInstanceNameFormat = "HefezopfSPHost-{0}-{1}";

        [Persisted]
        private Guid _ServiceApplicationId;

        /// <summary>Initializes a new instance.</summary>
        public HefezopfSPWindowsService() {        }

        /// <summary>Initializes a new instance.</summary>
        /// <param name="name">A string that contains a name for the persisted object.</param>
        /// <param name="farm">the local farm</param>
        public HefezopfSPWindowsService(string name, SPFarm farm) : base(name, farm)
        {
            //TODO.ThinkOf("remove lines or comment");
            //SPAdministrationWebApplication local = SPAdministrationWebApplication.Local;
            //SPProcessIdentity processIdentity = base.ProcessIdentity;
            //if (local != null)
            //{
            //    SPApplicationPool applicationPool = local.ApplicationPool;
            //    processIdentity.Username = applicationPool.Username;
            //    processIdentity.CurrentIdentityType = applicationPool.CurrentIdentityType;
            //}
            //else
            //{
            //    processIdentity.CurrentIdentityType = IdentityType.NetworkService;
            //    processIdentity.Username = null;
            //}
            //TODO.ThinkOf("remove lines or comment");
            base.ProcessIdentity.IsCredentialUpdateEnabled = false;
            base.ProcessIdentity.IsCredentialDeploymentEnabled = false;
        }

        /// <summary>Gets or sets the Id of the ServiceApplication.</summary>
        public Guid ServiceApplicationId { get { return this._ServiceApplicationId; } set { this._ServiceApplicationId = value; } }

        /// <summary>Gets the related ServiceApplication</summary>
        /// <returns>the related ServiceApplication or null.</returns>
        public HefezopfServiceApplication GetServiceApplication() {
            var service = HefezopfService.GetService();
            if ((object)service == null) { return null; }
            //
            HefezopfServiceApplication result = service.GetServiceApplication(this._ServiceApplicationId);
            return result;
        }

        /// <summary>Makes the changes to the local server that are needed before the object can be used.</summary>
        public override void Provision() {
            if (this.ServiceApplicationId == Guid.Empty) {
            } else {
                if (this.Status != SPObjectStatus.Provisioning) {
                    this.Status = SPObjectStatus.Provisioning;
                    this.Update();
                }
                //

                var otsService = HefezopfService.GetService();
                HefezopfServiceApplication serviceApplication = null;
                if ((object)otsService != null) {
                    serviceApplication = this.Farm.GetObject(this.ServiceApplicationId) as HefezopfServiceApplication;
                    //
                    if ((object)serviceApplication != null) {
                        var applicationPool = serviceApplication.ApplicationPool;
                        if ((object)applicationPool != null) {
                            if (!applicationPool.ProcessAccount.SecurityIdentifier.Equals(this.ProcessIdentity.ProcessAccount.SecurityIdentifier)) {
                                this.ProcessIdentity.ProcessAccount = applicationPool.ProcessAccount;
                                this.ProcessIdentity.Update();
                            }
                        }
                    }
                }
                //
                base.Provision();
                if (this.Status != SPObjectStatus.Online) {
                    this.Status = SPObjectStatus.Online;
                    this.Update();
                }
                {
                    var currentServiceInstances = new List<SPServiceInstance>();
                    var currentServers = new List<SPServer>();
                    var serversToProvision = new List<SPServer>();
                    if ((object)serviceApplication == null) {
#warning                        TODO.Now("remove all");
                    } else {
                        foreach (SPServiceInstance serviceInstance in this.Instances) {
                            if (serviceInstance.Server.Role != SPServerRole.Invalid) {
                                currentServiceInstances.Add(serviceInstance);
                                currentServers.Add(serviceInstance.Server);
                            }
                        }
#warning NOW                        serversToProvision.AddRange(serviceApplication.GetServersToProvision(currentServers.ToList(), this.Farm));
                    }
                    //Microsoft.SharePoint.Administration.SPWebServiceInstance.LocalAdministration.Server
                    if (serversToProvision.Count > 0) {
                        var serviceInstancesCurrent = new List<SPServiceInstance>();
                        var serversCurrent = new List<SPServer>();
                        foreach (var instance in this.Instances) {
                            serviceInstancesCurrent.Add(instance);
                            serversCurrent.Add(instance.Server);
                        }
                        var serversProvisioned = new List<SPServer>();
                        foreach (var serverToAdd in serversToProvision) {
                            if (serversCurrent.Contains(serverToAdd)) {
                                // nothing to do
                            } else {
                                // add the instance for the server and provision
                                var si = new HefezopfSPWindowsServiceInstance(string.Format(HostInstanceNameFormat, this.ServiceApplicationId.ToString("N"), serverToAdd.Name), serverToAdd, this);
                                si.Status = SPObjectStatus.Disabled;
                                si.Update();
                                si.Provision();
                                serversProvisioned.Add(serverToAdd);
                            }
                        }
                        //foreach (var si in this.Instances)
                        //{
                        //    if (!serversProvisioned.Contains(si.Server))
                        //    {
                        //        si.Provision();
                        //        serversProvisioned.Add(si.Server);
                        //    }
                        //}
                        foreach (var serviceInstance in serviceInstancesCurrent) {
                            if (serversToProvision.Contains(serviceInstance.Server)) {
                                //OK
                            } else {
                                if (serviceInstance.Status == SPObjectStatus.Online) {
                                    serviceInstance.Unprovision();
                                }
                            }
                        }
                    }
                }
                {
                    try {
                        this.ProcessIdentity.Provision();
                    } catch (Exception exc) {
                        System.Diagnostics.Trace.TraceError(exc.ToString());
                    }
                }
            }
        }

        /// <summary>Unprovision.</summary>
        public override void Unprovision() {
            base.Unprovision();
        }

        /// <summary>Gets the Typename.</summary>
        public override string TypeName => "Hefezopf Host Service";

        /// <summary>Gets the DisplayName.</summary>
        public override string DisplayName => "Hefezopf Host Service";

#warning NOW fix
#if false
  

        /// <summary>Get the name of the <see cref="HefezopfSPHostWindowsService"/> for the service application with the given id.</summary>
        /// <param name="serviceApplicationId">the <see cref="Microsoft.SharePoint.Administration.SPPersistedObject.Id"/> service application</param>
        /// <returns>the name</returns>
        public static string GetObjectNameOf(Guid serviceApplicationId) => string.Format(HostNameFormat, serviceApplicationId.ToString("N"));

        /// <summary>Gets an existing service .</summary>
        /// <param name="serviceApplicationId">the Id of the ServiceApplication</param>
        /// <returns>An instance of the Service or null.</returns>
        public static HefezopfSPHostWindowsService GetService(Guid serviceApplicationId)
        {
            HefezopfSPHostWindowsService service = HefezopfSPFactory.Instance.GetHefezopfSPHostWindowsService(serviceApplicationId);
            return service;
        }

        /// <summary>Gets an existing service or creates it if it doesn't exist.</summary>
        /// <param name="serviceApplicationId">the Id of the ServiceApplication</param>
        /// <returns>An instance of the Service.</returns>
        public static HefezopfSPHostWindowsService GetOrCreateService(Guid serviceApplicationId)
        {
            HefezopfSPHostWindowsService service = HefezopfSPFactory.Instance.GetHefezopfSPHostWindowsService(serviceApplicationId);
            if ((object)service == null)
            {
                service = HefezopfSPFactory.Instance.CreateHefezopfSPHostWindowsService(serviceApplicationId);
                service.Status = SPObjectStatus.Provisioning;
                service.Update();
            }
            return service;
        }

        

#endif
    }
}
