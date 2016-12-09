namespace Hefezopf.WindowsService.Shared {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ServiceProcess;

    public class HZServiceBaseWithArguments : ServiceBase, IHZServiceBaseWithArguments {
        IHZService _ServiceImplementation;

        public HZServiceBaseWithArguments(string serviceName, string defaultServiceName) {
            if (string.IsNullOrEmpty(serviceName)) {
                serviceName = defaultServiceName;
            }
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Launch the service for debugging proposes.
        /// </summary>
        /// <returns>a IDisposable to stop this service.</returns>
        public virtual IDisposable Debug() {
            this.EnsureServiceImplementation();
            var startArguments = this.GetStartArguments();
            this._ServiceImplementation?.Start(startArguments);
            return new StopDispose(this);
        }

        /// <summary>
        /// Parse the arguments
        /// </summary>
        /// <param name="args">the commandline, service arguments.</param>
        public virtual void SetArguments(IEnumerable<string> args) {
            this.Parse(args ?? new string[0]);
        }

        /// <summary>
        /// Called from the OS to start this service.
        /// Parse the args and start the service implementation.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args) {
            this.EnsureServiceImplementation();
            this.Parse(args ?? new string[0]);
            var startArguments = this.GetStartArguments();
            this._ServiceImplementation?.Start(startArguments);
        }

        /// <summary>
        /// Called from the OS to stop this service.
        /// </summary>
        protected override void OnStop() {
            this._ServiceImplementation?.Stop();
            using (var d = this._ServiceImplementation) { this._ServiceImplementation = null; }
        }

        /// <summary>
        /// Returns the IHZService - instance. May be it will be created first.
        /// </summary>
        /// <returns>the one and only IHZService - instance.</returns>
        protected IHZService EnsureServiceImplementation() {
            if (this._ServiceImplementation == null) {
                this._ServiceImplementation = this.CreateServiceImplementation();
                if ((object)this._ServiceImplementation == null) {
                    throw new InvalidOperationException("The result of CreateServiceImplementation should be null.");
                }
                this._ServiceImplementation.SetOwner(this);
            }
            return this._ServiceImplementation;
        }

        /// <summary>
        /// Creates a instance of your implementation.
        /// </summary>
        /// <returns>the instance - null will raise an error.</returns>
        protected virtual IHZService CreateServiceImplementation() {
            return default(IHZService);
        }

        /// <summary>
        /// Parse the commandline - if needed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Parse(IEnumerable<string> args) {
            // the Prefix - is reserved for you.
            //if (args == null || !args.Any()) { return; }
        }

        /// <summary>
        /// returns the commandline arguments or..
        /// </summary>
        /// <returns>what you want.</returns>
        protected virtual object GetStartArguments() {
            return null;
        }

        /// <summary>Stops and dispose the ServiceImplementation</summary>
        /// <param name="disposing">true for dispose</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.OnStop();
            } else {
                try {
                    this._ServiceImplementation?.Stop();
                } catch { }
                try {
                    using (var d = this._ServiceImplementation) { this._ServiceImplementation = null; }
                } catch { }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Helper class that calls stop.
        /// </summary>
        class StopDispose : IDisposable {
            private HZServiceBaseWithArguments _Owner;

            public StopDispose(HZServiceBaseWithArguments owner) {
                this._Owner = owner;
            }

            public void Dispose() {
                using (var d = this._Owner) {
                    this._Owner?.Stop();
                    this._Owner = null;
                }
            }
        }
    }
}
