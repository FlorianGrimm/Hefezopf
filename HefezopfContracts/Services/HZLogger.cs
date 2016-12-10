using System;

namespace Hefezopf.Contracts.Services {
    /// <summary>
    /// static for the current Logger.
    /// </summary>
    public static class HZLogger {
        private static IHZLogger _Current;

        /// <summary>
        /// Gets or sets the current logger.
        /// </summary>
        public static IHZLogger Current {
            get {
                if (_Current != null) {
                    return _Current;
                }
                _Current = new HZLoggerSystemDiagnostics();
                try {
                    return _Current = DI.DIService.GlobalFuncstructor.Resolve<IHZLogger>(null);
                } catch {
                    return _Current ?? (_Current = new HZLoggerSystemDiagnostics());
                }
            }
            set { _Current = value; }
        }

        /// <summary>
        /// Writes a trace to a file or the Microsoft SharePoint Foundation trace log.
        /// </summary>
        /// <param name="id">The application-defined identifier for the trace.</param>
        /// <param name="categoryName">The category of the trace.</param>
        /// <param name="traceLevel"> The severity of the trace. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>
        public static void Log(uint id, string categoryName, System.Diagnostics.TraceLevel traceLevel, string output, params object[] data) {
            Current.Log(id, categoryName, traceLevel, output, data);
        }

        /// <summary>
        /// log error
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="exception">exception</param>
        public static void CatchedError(string categoryName, Exception exception) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Error, exception.ToString());
        }

        /// <summary>
        /// log info
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="exception">exception</param>
        public static void CatchedInfo(string categoryName, Exception exception) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Info, exception.ToString());
        }

        /// <summary>
        /// log error
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="output">the message</param>
        /// <param name="data">message format arguments</param>
        public static void Error(string categoryName, string output, params object[] data) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Error, output, data);
        }
        /// <summary>
        /// log warning
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="output">the message</param>
        /// <param name="data">message format arguments</param>
        public static void Warning(string categoryName, string output, params object[] data) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Warning, output, data);
        }

        /// <summary>
        /// log verbose
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="output">the message</param>
        /// <param name="data">message format arguments</param>
        public static void Info(string categoryName, string output, params object[] data) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Info, output, data);
        }

        /// <summary>
        /// log verbose
        /// </summary>
        /// <param name="categoryName">the category</param>
        /// <param name="output">the message</param>
        /// <param name="data">message format arguments</param>
        public static void Verbose(string categoryName, string output, params object[] data) {
            Current.Log(0, categoryName, System.Diagnostics.TraceLevel.Verbose, output, data);
        }
    }
}
