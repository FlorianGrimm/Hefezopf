namespace Hefezopf.SharePoint.SPDiagnostics {
    using Microsoft.SharePoint.Administration;
    using Hefezopf.SPDiagnostics.Shared;
    using System.Collections.Generic;

    /// <summary>
    /// Settings for HefezopfDiagnosticsService
    /// </summary>
    public sealed class HZSPDiagnosticsServiceConfiguration : IHZSPDiagnosticsServiceConfiguration {
        public string AreaName => "Hefezopf";

        public string InstanceName => "HefezopfDiagnosticsService";

        public IEnumerable<string> CategoryNames => new string[] { Shared.Consts.CategoryCommon, Shared.Consts.CategoryDatabase, Shared.Consts.CategoryUI };

        public string DefaultCategoryName => Shared.Consts.CategoryCommon;
    }

    /// <summary>
    /// implementation HZSPDiagnosticsService
    /// </summary>
    [System.Runtime.InteropServices.Guid("0E0987C4-1651-487A-A624-7A9114075C3C")]
    public sealed class HZSPDiagnosticsService : HZSPDiagnosticsServiceBase<HZSPDiagnosticsService, HZSPDiagnosticsServiceConfiguration> {
        /// <summary>Initializes a new instance of the <see cref="HZSPDiagnosticsService"/> class.</summary>
        public HZSPDiagnosticsService() : base() { }

        /// <summary>Initializes a new instance of the <see cref="HZSPDiagnosticsService"/> class.</summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="parent">The local farm.</param>
        public HZSPDiagnosticsService(string name, SPFarm parent) : base(name, parent) { }
    }
}
