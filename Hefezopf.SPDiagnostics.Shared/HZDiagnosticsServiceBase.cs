namespace Hefezopf.SPDiagnostics.Shared
{
    using Microsoft.SharePoint.Administration;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// Provide a customizable SPDiagnosticsServiceBase
    /// </summary>
    /// <typeparam name="TInstance">The implementation type</typeparam>
    /// <typeparam name="TCfg">The configuration type.</typeparam>
    public class HZSPDiagnosticsServiceBase<TInstance, TCfg> : SPDiagnosticsServiceBase, IHZSPDiagnosticsServiceBase
        where TInstance : SPDiagnosticsServiceBase, IHZSPDiagnosticsServiceBase, new()
        where TCfg : class, IHZSPDiagnosticsServiceConfiguration, new() {

        private static readonly object _Lock = new object();

        private static TCfg _Cfg;
        private static TCfg GetCfg() => (_Cfg ?? (_Cfg = new TCfg()));
        private static string GetInstanceName() => GetCfg().InstanceName;
        private static string GetAreaName() => GetCfg().AreaName;
        private static IEnumerable<string> GetCategoryNames() => GetCfg().CategoryNames;
        private static string GetDefaultCategoryName() => GetCfg().DefaultCategoryName;

        /// <summary>Initializes a new instance of the <see cref="HZSPDiagnosticsServiceBase"/> class.</summary>
        protected HZSPDiagnosticsServiceBase() : base(GetInstanceName(), SPFarm.Local) { }

        /// <summary>Initializes a new instance of the <see cref="HZSPDiagnosticsServiceBase"/> class.</summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="parent">The local farm.</param>
        protected HZSPDiagnosticsServiceBase(string name, SPFarm parent) : base(name, parent) { }

        /// <summary>Determines whether other than the Farm has additional update access.</summary>
        /// <returns>true</returns>
        protected override bool HasAdditionalUpdateAccess() {
            // Without this SPDiagnosticsServiceBase.GetLocal<MyDiagnosticsService>()
            // throws a SecurityException, see
            // http://share2010.wordpress.com/tag/sppersistedobject/
            base.HasAdditionalUpdateAccess();
            return true;
        }

        /// <summary>Gets the local <see cref="T:HZSPDiagnosticsServiceBase"/>.</summary>
        /// <value>The local singleton.</value>
        public static TInstance GetLocal() {
            var result = SPFarm.Local.GetChild<TInstance>(GetInstanceName());
            if (result == null) {
                result = new TInstance();
            }
            return null;
        }

        /// <summary>Converts <see cref="t:Solvin.Ots.Contracts.LogCategory"/> to SPDiagnosticsCategory.</summary>
        /// <returns>the converted <see cref="t:Solvin.Ots.Contracts.LogCategory"/>.</returns>
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas() {
            var categoryNames = GetCategoryNames();
            var categories = categoryNames.Select(_ => new SPDiagnosticsCategory(_, TraceSeverity.Medium, EventSeverity.Error)).ToArray();
            var area = new SPDiagnosticsArea(GetAreaName(), categories);
            return new SPDiagnosticsArea[] { area };
        }

        /// <summary>
        /// Get the category <see cref="T:Microsoft.SharePoint.Administration.SPDiagnosticsCategory"/>
        /// for the category <see cref="T:LogCategory"/>
        /// </summary>
        /// <param name="category">the source category</param>
        /// <returns>the destination (SP-) category</returns>
        public SPDiagnosticsCategory GetCategory(string category) {
            {
                var result = this.Areas[GetAreaName()].Categories[category];
                if ((object)result != null) { return result; }
            }
            {
                var result = GetUnkownCategory(category);
                if ((object)result != null) { return result; }
            }
            {
                var result = this.Areas[GetAreaName()].Categories[GetDefaultCategoryName()];
                if ((object)result != null) { return result; }
            }
            {
                var result = this.Areas[GetAreaName()].Categories[0];
                if ((object)result != null) { return result; }
            }
            return null;
        }

        protected virtual SPDiagnosticsCategory GetUnkownCategory(string category) {
            return null;
        }

        private static string _CacheWssServicesRegistryKeyPath;
        private static string getWssServicesRegistryKeyPath() {
            if (_CacheWssServicesRegistryKeyPath != null) { return _CacheWssServicesRegistryKeyPath; }
            int major = typeof(SPFarm).Assembly.GetName().Version.Major;
            return _CacheWssServicesRegistryKeyPath = ($@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\{major}.0\WSS\Services");
        }
        /// <summary>Installs HZSPDiagnosticsServiceBase.</summary>
        /// <param name="farm">The farm - e.g properties.Definition.Farm.</param>
        public static void Install(SPFarm farm) {
            //Debugger.Break();
            if (farm == null) {
                farm = SPFarm.Local;
            }
            foreach (SPServer server in farm.Servers) {
                if (server.Role != SPServerRole.Invalid) {
                    try {
                        RegistryKey hklmRegistryKey = RegistryKey.OpenRemoteBaseKey(
                        RegistryHive.LocalMachine, server.Address);
                        // Create Registry key for integrating with SharePoint's Central Administration
                        // - Key name = namespace.classname
                        // - Value of AssemblyQualifiedName entry = assembly's strong name
                        RegistryKey wssServicesRegistryKey = hklmRegistryKey.OpenSubKey(
                            getWssServicesRegistryKeyPath(), true);
                        RegistryKey myDiagnosticsServiceRegistryKey = wssServicesRegistryKey.OpenSubKey(
                            typeof(TInstance).ToString());
                        if (myDiagnosticsServiceRegistryKey == null) {
                            myDiagnosticsServiceRegistryKey = wssServicesRegistryKey.CreateSubKey(
                                typeof(TInstance).ToString());
                            myDiagnosticsServiceRegistryKey.SetValue(
                                "AssemblyQualifiedName", typeof(TInstance).Assembly.FullName);
                        }
                    } catch (Exception exc) {
                        SPTraceLogError(exc, "HZSPDiagnosticsServiceBase-Install");
                    }
                }
            }
            try {
                //var local = (new TCfg()).GetLocal();
                var local = GetLocal();
                local.Update();
            } catch (Exception exc) {
                SPTraceLogError(exc, "HZSPDiagnosticsServiceBase-Install");
            }
        } // FeatureActivated()

        /// <summary>Uninstalls HZSPDiagnosticsServiceBase.</summary>
        /// <param name="farm">The farm - e.g properties.Definition.Farm.</param>
        public static void Uninstall(SPFarm farm) {
            //Debugger.Break();
            if (farm == null) {
                farm = SPFarm.Local;
            }
            foreach (SPServer server in farm.Servers) {
                if (server.Role != SPServerRole.Invalid) {
                    try {
                        RegistryKey hklmRegistryKey = RegistryKey.OpenRemoteBaseKey(
                        RegistryHive.LocalMachine, server.Address);
                        // - Remove Registry key for integrating with SharePoint's Central Administration
                        RegistryKey wssServicesRegistryKey = hklmRegistryKey.OpenSubKey(
                            getWssServicesRegistryKeyPath(), true);
                        RegistryKey myDiagnosticsServiceRegistryKey
                            = wssServicesRegistryKey.OpenSubKey(typeof(TInstance).ToString());
                        if (myDiagnosticsServiceRegistryKey != null) {
                            wssServicesRegistryKey.DeleteSubKey(typeof(TInstance).ToString());
                        }
                    } catch (Exception exc) {
                        SPTraceLogError(exc, "HZSPDiagnosticsServiceBase-Uninstall");
                    }
                }
            }
            try {
                //var local = (new TCfg()).GetLocal();
                var local = GetLocal();
                var localId = local.Id;
                local = null;
                farm.Services.Remove(localId);
            } catch (Exception exc) {
                SPTraceLogError(exc, "HZSPDiagnosticsServiceBase-Uninstall");
            }
        } // FeatureDeactivating()

        /// <summary>
        /// Writes an event to the Windows application event log, if the event severity is above the configured threshold for the SPDiagnosticsCategory.
        /// </summary>
        /// <param name="id">The application-defined identifier for the event.</param>
        /// <param name="categoryName">The category of the event.</param>
        /// <param name="severity"> The severity of the event. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>
        /// <remarks>
        /// You must use a category that is recognized by the service. For an example showing how to query a diagnostics services for areas and categories, see the Areas property.
        /// Keep in mind that if you pass a value in the severity parameter that is less than the currently configured value for the category's EventSeverity property, the trace is not written to the log.
        /// </remarks>
        public static void WriteEvent(ushort id, string categoryName, EventSeverity severity, string output, params object[] data) {
            var that = GetLocal();
            SPDiagnosticsCategory category = that.GetCategory(categoryName);
            that.WriteEvent(id, category, severity, output, data);
        }
        /// <summary>
        /// Writes a trace to the Microsoft SharePoint Foundation trace log.
        /// </summary>
        /// <param name="id">The application-defined identifier for the trace.</param>
        /// <param name="categoryName">The category of the trace.</param>
        /// <param name="severity"> The severity of the trace. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>
        public static void WriteTrace(uint id, string categoryName, TraceSeverity severity, string output, params object[] data) {
            var that = GetLocal();
            SPDiagnosticsCategory category = that.GetCategory(categoryName);
            that.WriteTrace(id, category, severity, output, data);
        }

        /// <summary>
        /// INTERNAL USE - NOT FOR YOU
        /// </summary>
        public static void SPTraceLogError(Exception ex) {
            SPDiagnosticsService.Local.WriteTrace(
                0,
                new SPDiagnosticsCategory("Dev Events", TraceSeverity.High, EventSeverity.Error),
                TraceSeverity.Unexpected,
                ex.Message,
                ex.StackTrace);
        }

        /// <summary>
        /// INTERNAL USE - NOT FOR YOU
        /// </summary>
        public static void SPTraceLogError(Exception ex, string keyNote) {
            SPDiagnosticsService.Local.WriteTrace(
                0,
                new SPDiagnosticsCategory("Dev Events", TraceSeverity.High, EventSeverity.Error),
                TraceSeverity.Unexpected, keyNote + " : " + ex.Message, ex.StackTrace);
        }
    }

    /// <summary>
    /// helper only
    /// </summary>
    public interface IHZSPDiagnosticsServiceBase {
        /// <summary>
        /// Get the category <see cref="T:Microsoft.SharePoint.Administration.SPDiagnosticsCategory"/>
        /// for the category <see cref="T:LogCategory"/>
        /// </summary>
        /// <param name="category">the source category</param>
        /// <returns>the destination (SP-) category</returns>
        SPDiagnosticsCategory GetCategory(string category);
    }
}

