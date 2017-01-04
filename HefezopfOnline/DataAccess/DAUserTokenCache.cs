

namespace HefezopfOnline.DataAccess {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Gsaelzbrot.Library;
    using System.Data.SqlClient;

    public class DAUserTokenCache {
        private IGsbConnection _Connection;

        public DAUserTokenCache(Gsaelzbrot.Library.IGsbConnection connection) {
            this._Connection = connection;
        }


#if false
        public GsbCommandStep<Parameters_Hefezopf_ApplicationSettings_GetByHostName> GetReadCommand() {
            //var commandParameters = new GsbCommandParameters_Hefezopf_ApplicationSettings_GetByHostName();
            var command = this._Connection.CreateCommand<GsbCommandStep<Parameters_Hefezopf_ApplicationSettings_GetByHostName>>(
                that => that.CommandTypedParameters = new GsbCommandParameters<Parameters_Hefezopf_ApplicationSettings_GetByHostName>() {
                    ActionSetTypedParameters = (commandTypedParameters, value) => {
                        commandTypedParameters.Set("@HostName", value.HostName, System.Data.SqlDbType.NVarChar, 200);
                    }
                }
                );
            return command;
        }
        public void Read(string hostName) {
            this._Connection.Open();
            var cmd = this.GetReadCommand();
            using (var cmd = this.GetReadCommand()) {

            }

        }
    }
    public class Parameters_Hefezopf_ApplicationSettings_GetByHostName {
        public string HostName { get; set; }
    }

    public class GsbCommandParameters_Hefezopf_ApplicationSettings_GetByHostName
        : GsbCommandParameters<Parameters_Hefezopf_ApplicationSettings_GetByHostName> {
        public override void SetTypedParameters(Parameters_Hefezopf_ApplicationSettings_GetByHostName value) {
            base.SetTypedParameters(value);
            this.Set("@HostName", value.HostName, System.Data.SqlDbType.NVarChar, 200);
        }
    }
    public class Hefezopf_ApplicationSettings_GetByHostName: GsbCommandStep<string> {
        public Hefezopf_ApplicationSettings_GetByHostName()
            : base("[Hefezopf].[ApplicationSettings_GetByHostName]", new GsbCommandParameters<string>()) {
        }
        //private SqlParameter _ParameterHostName;
        //public override void CreateParameters() {
        //    base.CreateParameters();
        //    this._ParameterHostName = this.CreateParameter("@HostName", System.Data.SqlDbType.NVarChar, 200);
        //}

        //public SqlParameter CreateParameter(string parameterName, System.Data.SqlDbType sqlDbType, int size) {
        //    var result = new SqlParameter();
        //    result.ParameterName = parameterName;
        //    result.SqlDbType = sqlDbType;
        //    result.Size = size;
        //    this.Command.Parameters.Add(parameterName);
        //    return result;
        //}

        //public override void SetParameters() {
        //    base.SetParameters();
        //    this._ParameterHostName.Value = ""
        //}

#endif
    }
}

namespace HefezopfOnline.DataAccess {
    using System;

    public class UserTokenCache {
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime ModifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        //public DateTime ModifiedAt { get;  set; }
        public long VersionNumber { get; set; }
    }
}

namespace HefezopfOnline.DataAccess.Converter {
    using Gsaelzbrot.Library;
    public partial class DAUserTokenCacheConverter {
        public static UserTokenCache Convert_HefezopfOnline_UserTokenCache(GsbRecordValues record) {
            var result = new UserTokenCache();
            Convert_HefezopfOnline_UserTokenCache(record, result, 0);
            return result;
        }
        public static void Convert_HefezopfOnline_UserTokenCache(GsbRecordValues record, UserTokenCache result, int offset) {
            result.WebUserUniqueId = record.ReadString(0 + offset);
            result.CacheBits = record.ReadVarBinary(1 + offset);
            result.CreatedAt = record.ReadDateTime(2 + offset);
            result.ModifiedAt = record.ReadDateTime(3 + offset);
            result.VersionNumber = record.ReadBigInt(4 + offset);
        }
    }
}
namespace HefezopfOnline.DataAccess {
    using System;

    public class ApplicationSettings {
        public string HostName { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
        public string SettingName { get; internal set; }
        public string SettingValue { get; internal set; }
        public long ApplicationSettingsHost_VersionNumber { get; internal set; }
        public long ApplicationSettingsValue_VersionNumber { get; internal set; }
    }
}
namespace HefezopfOnline.DataAccess.Parameters {
    public class Hefezopf_ApplicationSettings_GetByHostName {
        public string HostName { get; set; }
    }
}
namespace HefezopfOnline.DataAccess.Converter {
    using Gsaelzbrot.Library;
    public partial class DAUserTokenCacheConverter {
        public static ApplicationSettings Convert_Hefezopf_ApplicationSettings(GsbRecordValues record) {
            var result = new ApplicationSettings();
            Convert_Hefezopf_ApplicationSettings(record, result, 0);
            return result;
        }
        public static void Convert_Hefezopf_ApplicationSettings(GsbRecordValues record, ApplicationSettings result, int offset) {
            result.HostName = record.ReadString(0 + offset);
            result.SettingName = record.ReadString(1 + offset);
            result.SettingValue = record.ReadString(2 + offset);
            result.ApplicationSettingsHost_VersionNumber = record.ReadBigInt(3 + offset);
            result.ApplicationSettingsValue_VersionNumber = record.ReadBigInt(4 + offset);
        }
    }
}