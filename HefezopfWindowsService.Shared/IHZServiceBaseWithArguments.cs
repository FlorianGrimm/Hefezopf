namespace Hefezopf.WindowsService.Shared {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extends the <see cref="System.ServiceProcess.ServiceBase"/>.
    /// </summary>
    public interface IHZServiceBaseWithArguments {
        /// <summary>
        /// set the arguments from the commandline, registry....
        /// </summary>
        /// <param name="args">the arguments. Use - as prefix</param>
        void SetArguments(IEnumerable<string> args);

        /// <summary>
        /// Launch the service without being a service.
        /// </summary>
        /// <returns>Dispose the resuklt to stop.</returns>
        IDisposable Debug();
    }   
}
