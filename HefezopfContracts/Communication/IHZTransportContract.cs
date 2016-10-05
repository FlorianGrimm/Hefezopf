using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefezopf.Contracts.Communication
{
    [System.ServiceModel.ServiceContract]
    public interface IHZTransportContract
    {
        [System.ServiceModel.OperationContract]
        string Execute(string request);
        //string Execute(string target, string action, string parameter);

        [System.ServiceModel.OperationContract]
        void ExecuteQueue(string target, string action, string parameter);
    }
}
