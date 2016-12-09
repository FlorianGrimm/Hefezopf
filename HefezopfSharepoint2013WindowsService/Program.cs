namespace HefezopfSharepoint2013WindowsService {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using Hefezopf.WindowsService.Shared;

    /// <summary>
    /// The main entry point for the service.
    /// </summary>
    public static class Program {
        /// <summary>
        /// The main entry point for the service.
        /// </summary>
        public static int Main(string[] args) {
            var bootingService = new HZBootingService(
                () => typeof(HZService).Assembly,
                (serviceName) => new HZService(serviceName),
                Consts.DefaultServiceName,
                Consts.EventSourceName);
            return bootingService.Main(args);
        }
    }
}
