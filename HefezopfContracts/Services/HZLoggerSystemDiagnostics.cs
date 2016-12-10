using System;
using System.Diagnostics;

namespace Hefezopf.Contracts.Services {
    /// <summary>
    /// write to System.Diagnostics
    /// </summary>
    public sealed class HZLoggerSystemDiagnostics : IHZLogger {
        /// <summary>
        /// Log it
        /// </summary>
        /// <param name="id">The application-defined identifier for the trace.</param>
        /// <param name="categoryName">The category of the trace.</param>
        /// <param name="traceLevel"> The severity of the trace. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>
        public void Log(uint id, string categoryName, TraceLevel traceLevel, string output, params object[] data) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format(output, data), categoryName);
#else
            switch (traceLevel) {
                case TraceLevel.Off:
                    break;
                case TraceLevel.Error:
                    System.Diagnostics.Trace.TraceError(string.Format(output, data), categoryName);
                    break;
                case TraceLevel.Warning:
                    System.Diagnostics.Trace.TraceWarning(string.Format(output, data), categoryName);
                    break;
                case TraceLevel.Info:
                    System.Diagnostics.Trace.TraceInformation(string.Format(output, data), categoryName);
                    break;
                case TraceLevel.Verbose:
                    System.Diagnostics.Trace.TraceInformation(string.Format(output, data), categoryName);
                    break;
                default:
                    System.Diagnostics.Trace.TraceInformation(string.Format(output, data), categoryName);
                    break;
            }
#endif
        }
    }
}
