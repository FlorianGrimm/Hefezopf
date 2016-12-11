using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.LocalWebHost {
    public class MainModel : System.ComponentModel.INotifyPropertyChanged {
        private int _Port;

        public MainModel() {
        }

        public int Port { get { return this._Port; } set { this._Port = value; this.OnPropertyChanged(nameof(this.Port)); } }

        public void OnPropertyChanged(string name) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Start() {
        }
    }
}
