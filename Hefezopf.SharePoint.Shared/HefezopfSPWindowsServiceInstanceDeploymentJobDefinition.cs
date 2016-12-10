namespace Hefezopf.SharePoint.Shared {
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [Guid("7A9E3FB8-A52B-4C68-9731-486517B68E56")]
    public class HefezopfSPWindowsServiceInstanceDeploymentJobDefinition : SPAdministrationServiceJobDefinition {
        internal const string ModeProvision = "Provision";
        internal const string ModeUnprovision = "Unprovision";
        [Persisted]
        private HefezopfSPWindowsServiceInstance _Instance;
        [Persisted]
        private string _Mode;

        /// <summary>Gets the displayname.</summary>
        public override string DisplayName => SPResource.GetString("CertificateStoreDeploymentJobTitle", new object[]
            {
                this._Instance.DisplayName
            });

        /// <summary>Constructor.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HefezopfSPWindowsServiceInstanceDeploymentJobDefinition()
        {
        }

        /// <summary>Contstructor.</summary>
        public HefezopfSPWindowsServiceInstanceDeploymentJobDefinition(
            HefezopfSPWindowsServiceInstance instance, 
            HefezopfSPWindowsService service, 
            SPServer server, 
            SPJobLockType lockType, 
            string mode)
            : base(HefezopfSPWindowsServiceInstanceDeploymentJobDefinition.NameFromParams(instance), service, server, SPJobLockType.None)
        {
            this._Instance = instance;
            this._Mode = mode;
            base.Schedule = new SPOneTimeSchedule(System.DateTime.Now);
        }

        /// <summary>Calculate a name.</summary>
        public static string NameFromParams(HefezopfSPWindowsServiceInstance instance) => "job-hefezopf-deploy" + instance.Name;

        /// <summary>Update or delete.</summary>
        public override void Update()
        {
            SPTimerService timerService = base.Farm.TimerService;
            if (null != timerService && !base.WasCreated)
            {
                HefezopfSPWindowsServiceInstanceDeploymentJobDefinition value = timerService.JobDefinitions.GetValue<HefezopfSPWindowsServiceInstanceDeploymentJobDefinition>(HefezopfSPWindowsServiceInstanceDeploymentJobDefinition.NameFromParams(this._Instance));
                if (null != value)
                {
                    value.Delete();
                }
            }
            base.Update();
        }

        /// <summary>Call Un-Provision.</summary>
        public override void Execute(System.Guid targetInstanceId)
        {
            if (string.IsNullOrEmpty(this._Mode)) { this._Mode = ModeProvision; }
            if (string.Equals(this._Mode, ModeProvision))
            {
                this._Instance.ProvisionInternal();
            }
            if (string.Equals(this._Mode, ModeUnprovision))
            {
                this._Instance.UnprovisionInternal();
            }
        }
    }
}
