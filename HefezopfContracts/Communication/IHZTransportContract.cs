using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.Communication
{
    /// <summary>
    /// This service provide a fast way to communicate.
    /// </summary>
    [System.ServiceModel.ServiceContract(Namespace = ContractConsts.Namespace)]
    public interface IHZTransportContract
    {
        /// <summary>
        /// Execute one action.
        /// </summary>
        /// <param name="request">The request jsonfied.</param>
        /// <returns>The responce jsonfied.</returns>
        [System.ServiceModel.OperationContract]
        string Execute(string request);

        /// <summary>
        /// Execute many actions.
        /// </summary>
        /// <param name="requests">A list of request jsonfied.</param>
        /// <returns>A list of responces jsonfied.</returns>
        [System.ServiceModel.OperationContract]
        string[] ExecuteMany(string[] requests);

        /// <summary>
        /// Execute one action queued.
        /// </summary>
        /// <param name="request">The request jsonfied.</param>
        /// <returns>The responce jsonfied.</returns>
        [System.ServiceModel.OperationContract]
        string ExecuteQueue(string request);
    }
}
