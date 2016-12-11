namespace Hefezopf.WebHost {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Owin.Hosting;

    public class OwinService : IDisposable {
        private IDisposable _WebApp;
        private bool _IsDisposed;

        public OwinService() {
        }
        public void Start() {
            string baseUri = "http://localhost:8080";
            this._WebApp = WebApp.Start<Startup>(baseUri);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_IsDisposed) {
                using (var webApp = this._WebApp) {
                    if (disposing) {
                        //
                    }
                    this._WebApp = null;
                }
                _IsDisposed = true;
            } else {
                using (var webApp = this._WebApp) {
                    this._WebApp = null;
                }
            }
        }

        ~OwinService() { Dispose(false); }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    }
}
