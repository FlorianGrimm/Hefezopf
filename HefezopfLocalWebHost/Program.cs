using System;
using System.Reflection;

namespace Hefezopf.LocalWebHost {
    /// <summary>
    /// Application Entry Point.
    /// </summary>
    public static class Program {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main() {
            try {
                using (var assemblyInResource = new Hefezopf.LocalWebHost.Assembly.HZAssemblyInResource()) {
                    assemblyInResource.ExtractTo("bin");
                }
                // invoke Hefezopf.LocalWebHost.App.Main();
                var assembly = typeof(Hefezopf.LocalWebHost.Program).Assembly;
                var typeApp = assembly.GetType("Hefezopf.LocalWebHost.App");
                var method = typeApp.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, null);
            } catch (Exception exception) {
                System.Diagnostics.Trace.WriteLine(exception.ToString());
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
        }
    }
}
