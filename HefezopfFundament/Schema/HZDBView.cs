using System;
using System.Collections.Generic;
using Gsaelzbrot.Library;

namespace Hefezopf.Fundament.Schema {
    /// <summary>
    /// Represent a view
    /// </summary>
    public class HZDBView
        : HZDBObjectWithColumnAndIndex
        , GsbView {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZDBView"/> class.
        /// </summary>
        public HZDBView() {
        }

        public bool IsSchemaBound {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool ReturnsViewMetadata {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public string TextBody {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public string TextHeader {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        IList<GsbColumn> GsbView.Columns {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        IList<GsbIndex> GsbView.Indexes {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }
    }
}