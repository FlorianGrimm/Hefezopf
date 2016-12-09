namespace Hefezopf.Contracts.Communication {
    using System.Runtime.Serialization;

    /// <summary>
    /// Responce of the service
    /// </summary>
    [DataContract(Namespace = ContractConsts.Namespace)]
    public class HZServiceResponce {
        /// <summary>
        /// Initializes a new instance of the <see cref="HZServiceResponce"/> class.
        /// </summary>
        public HZServiceResponce() { }

        /// <summary>
        /// Gets or sets the Identity.
        /// </summary>
        [DataMember]
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets resultType.
        /// </summary>
        [DataMember]
        public string ResultType { get; set; }

        /// <summary>
        /// Gets or sets result.
        /// </summary>
        [DataMember]
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets errorType.
        /// </summary>
        [DataMember]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets errorMessage.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}