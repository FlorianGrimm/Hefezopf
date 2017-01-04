namespace HefezopfOnline.Models {
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

    public class ApplicationSettings {
        private readonly Dictionary<string, string[]> _Items;
        public ApplicationSettings() {
            this._Items = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);
        }

        public string[] this[string key]{
            get {
                string[] result = null;
                string[] values = null;
                if (_Items.TryGetValue(key, out values)) {
                    result = new string[values.Length];
                    values.CopyTo(result, 0);
                } else {
                    result = new string[0];
                }
                return result;
            }
        }

        public void ReadAppSettings() {
            NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;
            foreach (var key in appSettings.AllKeys) {
                var values = appSettings.GetValues(key);
                this._Items[key] = values;
            }
        }

        public void ReadDatabaseSettings(Gsaelzbrot.Library.IGsbConnection connection) {
            //connection.CreateCommand
        }
    }
}