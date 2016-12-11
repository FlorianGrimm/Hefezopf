using Hefezopf.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.SharePoint.Administration;

namespace Hefezopf.SPDiagnostics.Shared {
    public class HZSPLoggerBase<TInstance, TCfg>
        : IHZLogger
        where TInstance : SPDiagnosticsServiceBase, IHZSPDiagnosticsServiceBase, new()
        where TCfg : class, IHZSPDiagnosticsServiceConfiguration, new() {
        /// <summary>
        /// 
        /// </summary>
        protected HZSPLoggerBase() {
        }

        /// <summary>
        /// Get the local instance.
        /// </summary>
        /// <returns>the local instance.</returns>
        public virtual TInstance GetLocal() { return default(TInstance); }

        /// <summary>
        /// Writes a trace to the Microsoft SharePoint Foundation trace log.
        /// </summary>
        /// <param name="id">The application-defined identifier for the trace.</param>
        /// <param name="categoryName">The category of the trace.</param>
        /// <param name="traceLevel"> The severity of the trace. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>        
        public void Log(uint id, string categoryName, TraceLevel traceLevel, string output, params object[] data) {
            var local = this.GetLocal();
            local.Log(id, categoryName, traceLevel, output, data);
        }
    }
}
