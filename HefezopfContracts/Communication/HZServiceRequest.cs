using System.Runtime.Serialization;

namespace Hefezopf.Contracts.Communication
{
    [DataContract(Namespace = ContractConsts.Namespace)]
    public class HZServiceRequest
    {
        public HZServiceRequest() { }

        [DataMember]
        public string Identity { get; set; }

        [DataMember]
        public string Target { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string ParameterType { get; set; }

        [DataMember]
        public string Parameter { get; set; }
    }
}