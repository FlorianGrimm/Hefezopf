namespace Hefezopf.Fundament.Schema {
    using System;
    using Gsaelzbrot.Library;

    internal class HZDatabaseGsbModelFactory : IGsbModelFactory {
        private HZDatabase _Database;

        public HZDatabaseGsbModelFactory(HZDatabase database) {
            if (database == null) { throw new ArgumentNullException(); }
            this._Database = database;
        }

        public IGsbSchema CreateGsbSchema() {
            var result = new HZDBSchema();
            return result;
        }

        public GsbColumn CreateGsbColumn() {
            throw new NotImplementedException();
        }

        public GsbDataType CreateGsbDataType() {
            throw new NotImplementedException();
        }

        public GsbFunction CreateGsbFunction() {
            throw new NotImplementedException();
        }

        public GsbIndex CreateGsbIndex() {
            throw new NotImplementedException();
        }

        public GsbIndexedColumn CreateGsbIndexedColumn() {
            throw new NotImplementedException();
        }

        public GsbParameter CreateGsbParameter() {
            throw new NotImplementedException();
        }

        public GsbStoredProcedure CreateGsbStoredProcedure() {
            throw new NotImplementedException();
        }

        public GsbTable CreateGsbTable() {
            throw new NotImplementedException();
        }

        public GsbView CreateGsbView() {
            throw new NotImplementedException();
        }
    }
}