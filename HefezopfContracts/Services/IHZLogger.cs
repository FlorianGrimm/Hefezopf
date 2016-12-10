namespace Hefezopf.Contracts.Services {
    /// <summary>
    /// defines logging.
    /// </summary>
    public interface IHZLogger {
        /// <summary>
        /// Writes a trace to a file or the Microsoft SharePoint Foundation trace log.
        /// </summary>
        /// <param name="id">The application-defined identifier for the trace.</param>
        /// <param name="categoryName">The category of the trace.</param>
        /// <param name="traceLevel"> The severity of the trace. </param>
        /// <param name="output">The message. Optionally, the message may contain format placeholders so that the string can be passed to System.String.Format(string, Object[]) for formatting.</param>
        /// <param name="data">The optional items to be replaced into the message format string.</param>
        void Log(uint id, string categoryName, System.Diagnostics.TraceLevel traceLevel, string output, params object[] data);
    }
}
