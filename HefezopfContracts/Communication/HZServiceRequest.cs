namespace Hefezopf.Contracts.Communication {
    using System.Runtime.Serialization;

    /// <summary>
    /// Request of the service.
    /// </summary>
    [DataContract(Namespace = ContractConsts.Namespace)]
    public class HZServiceRequest {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZServiceRequest"/> class.
        /// </summary>
        public HZServiceRequest() { }

        /// <summary>
        /// Gets or sets identity.
        /// </summary>
        [DataMember]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets target.
        /// </summary>
        [DataMember]
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        [DataMember]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets parameterType.
        /// </summary>
        [DataMember]
        public string ParameterType { get; set; }

        /// <summary>
        /// Gets or sets parameter.
        /// </summary>
        [DataMember]
        public string Parameter { get; set; }
    }
}