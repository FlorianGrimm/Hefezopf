namespace Hefezopf.SPDiagnostics.Shared
{    
    using System.Collections.Generic;

    public interface IHZSPDiagnosticsServiceConfiguration {
        string InstanceName { get; }
        string AreaName { get; }
        IEnumerable<string> CategoryNames { get; }
        string DefaultCategoryName { get; }
    }
}
