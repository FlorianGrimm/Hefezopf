namespace HefezopfWindowsService.Shared {
    using System;
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

        public IDisposable Debug(IEnumerable<string> args) {
            this.Parse(args ?? new string[0]);
            this._ServiceImplementation = CreateServiceImplementation();
            var startArguments = this.GetStartArguments();
            this._ServiceImplementation?.Start(startArguments);
            return new StopDispose(this);
        }

        public void SetArguments(IEnumerable<string> args) {
            this.Parse(args ?? new string[0]);
        }

        protected override void OnStart(string[] args) {
            this.Parse(args ?? new string[0]);
            var startArguments = this.GetStartArguments();
            this._ServiceImplementation?.Start(startArguments);
        }

        protected override void OnStop() {
            this._ServiceImplementation?.Stop();
            using (var d = this._ServiceImplementation) { this._ServiceImplementation = null; }
        }

        protected virtual IHZService CreateServiceImplementation() {
            return default(IHZService);
        }

        protected virtual void Parse(IEnumerable<string> args) {
            // the Prefix - is reserved for you.
        }

        protected virtual object GetStartArguments() {
            return null;
        }
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
        class StopDispose : IDisposable {
            private HZServiceBaseWithArguments _Owner;

            public StopDispose(HZServiceBaseWithArguments owner) {
                this._Owner = owner;
            }

            public void Dispose() {
                this._Owner.Stop();
                this._Owner.Dispose();
            }
        }
    }
}
