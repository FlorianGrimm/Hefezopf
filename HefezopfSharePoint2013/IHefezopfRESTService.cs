// Hefezopf
// MIT License
// Copyright (c) 2016 Florian GRimm

namespace Hefezopf.SharePoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The REST Service Contract.
    /// </summary>
    [ServiceContract(Namespace = Hefezopf.Contracts.ContractConsts.Namespace)]
    public interface IHefezopfRESTService
    {
        //[WebGet(UriTemplate = "/Execute/{request}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [System.ServiceModel.OperationContract]
        [WebInvoke(UriTemplate = "/ExecuteMany", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string Execute(string request);

        [System.ServiceModel.OperationContract]
        [WebInvoke(UriTemplate = "/ExecuteMany", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string[] ExecuteMany(string[] requests);

        //[WebGet(UriTemplate = "/Execute/{request}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [System.ServiceModel.OperationContract]
        [WebInvoke(UriTemplate = "/ExecuteQueue", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string ExecuteQueue(string request);

        ///// <summary>
        ///// Returns a hello world string.
        ///// </summary>
        ///// <param name="helloWorld">An input string of text.</param>
        ///// <returns>A string of text echoing the input value.</returns>
        //[OperationContract, WebGet(UriTemplate = "/HelloWorld/{helloWorld}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        //string HelloWorld(string helloWorld);

        ///// <summary>
        ///// Returns a hello world string from the database.
        ///// </summary>
        ///// <param name="helloWorld">An input string of text.</param>
        ///// <returns>A string of text echoing the input value.</returns>
        //[OperationContract, WebGet(UriTemplate = "/HelloWorldFromDatabase/{helloWorld}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        //string HelloWorldFromDatabase(string helloWorld);
    }
}
