using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.Communication
{
    [System.ServiceModel.ServiceContract(Namespace = ContractConsts.Namespace)]
    public interface IHZServiceContract
    {
        [System.ServiceModel.OperationContract]
        HZServiceResponce ExecuteOneAction(HZServiceRequest request);

        [System.ServiceModel.OperationContract]
        HZServiceResponce[] ExecuteManyActions(HZServiceRequest[] requests);

        [System.ServiceModel.OperationContract]
        HZServiceResponce ExecuteOneActionQueued(HZServiceRequest request);
    }
}
