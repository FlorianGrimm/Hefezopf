using System.Runtime.Serialization;

namespace Hefezopf.Contracts.Communication
{
    [DataContract]
    public class HZServiceResponce
    {
        public HZServiceResponce() { }

        [DataMember]
        public string Identity { get; set; }

        [DataMember]
        public string ResultType { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string ErrorType { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}