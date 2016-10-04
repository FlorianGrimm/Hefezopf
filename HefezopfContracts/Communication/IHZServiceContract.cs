using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.Communication
{
    [System.ServiceModel.ServiceContract]
    public interface IHZServiceContract
    {
        [System.ServiceModel.OperationContract]
        HZServiceResponce Execute(HZServiceRequest request);

        [System.ServiceModel.OperationContract]
        HZServiceResponce ExecuteMany(HZServiceRequest request);
    }
}
