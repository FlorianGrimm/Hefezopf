namespace Hefezopf.Contracts.Communication {
    /// <summary>
    /// TODO
    /// </summary>
    [System.ServiceModel.ServiceContract(Namespace = ContractConsts.Namespace)]
    public interface IHZServiceContract {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The responce</returns>
        [System.ServiceModel.OperationContract]
        HZServiceResponce ExecuteOneAction(HZServiceRequest request);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>The responces</returns>
        [System.ServiceModel.OperationContract]
        HZServiceResponce[] ExecuteManyActions(HZServiceRequest[] requests);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The responce</returns>
        [System.ServiceModel.OperationContract]
        HZServiceResponce ExecuteOneActionQueued(HZServiceRequest request);
    }
}
