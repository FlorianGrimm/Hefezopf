namespace Hefezopf.WindowsService.Shared {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// provides a simple service interaction.
    /// </summary>
    public interface IHZService : IDisposable {
        void SetOwner(System.ServiceProcess.ServiceBase owner);

        void Start(object args);

        void Stop();
    }
}
