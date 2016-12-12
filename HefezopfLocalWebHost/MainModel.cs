namespace Hefezopf.LocalWebHost {
    using Hefezopf.WebHost;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// MainModel
    /// </summary>
    public class MainModel : System.ComponentModel.INotifyPropertyChanged, System.IDisposable {
        private int _Port;
        private IDisposable _OwinService;
        private bool _IsDisposed;

        public MainModel() {
        }

        public int Port { get { return this._Port; } set { this._Port = value; this.OnPropertyChanged(nameof(this.Port)); } }

        public bool IsRunning { get { return this._OwinService != null; } set { this.OnPropertyChanged(nameof(this.IsRunning)); } }

        public void OnPropertyChanged(string name) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Start() {
            {
                var owinService = new OwinService();
                owinService.Start();
                this._OwinService = owinService;
            }
            this.IsRunning = true;
        }

        public void Stop() {
            using (var owinService = this._OwinService) {
                this._OwinService = null;
            }
            this.IsRunning = false;
        }


        protected virtual void Dispose(bool disposing) {
            if (!this._IsDisposed) {
                using (var d = this._OwinService) {
                    if (disposing) {
                        this._OwinService?.Dispose();
                    }

                    this._OwinService = null;
                }
                this._IsDisposed = true;
            }
            this.IsRunning = false;
        }

        ~MainModel() { this.Dispose(false); }

        public void Dispose() { this.Dispose(true); System.GC.SuppressFinalize(this); }
    }
}
