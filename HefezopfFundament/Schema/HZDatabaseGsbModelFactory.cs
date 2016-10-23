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
            result.Database = this._Database;
            return result;
        }

        public IGsbColumn CreateGsbColumn() {
            throw new NotImplementedException();
        }

        public IGsbDataType CreateGsbDataType() {
            throw new NotImplementedException();
        }

        public IGsbFunction CreateGsbFunction() {
            throw new NotImplementedException();
        }

        public IGsbIndex CreateGsbIndex() {
            throw new NotImplementedException();
        }

        public IGsbIndexedColumn CreateGsbIndexedColumn() {
            throw new NotImplementedException();
        }

        public IGsbParameter CreateGsbParameter() {
            throw new NotImplementedException();
        }


        public IGsbStoredProcedure CreateGsbStoredProcedure() {
            throw new NotImplementedException();
        }

        public IGsbTable CreateGsbTable() {
            throw new NotImplementedException();
        }

        public IGsbView CreateGsbView() {
            throw new NotImplementedException();
        }
    }
}