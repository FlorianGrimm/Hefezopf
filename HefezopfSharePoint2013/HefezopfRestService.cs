namespace Hefezopf.SharePoint
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using Microsoft.SharePoint.Administration;
    using Hefezopf.Service;
    using System.ServiceModel.Web;

    /// <summary>
    /// The REST Service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [System.Runtime.InteropServices.Guid("a42247d1-91e4-40f6-9653-0daa6667762a")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by the WCF runtime automatically.")]
    public class HefezopfRestService : IHelloWorldRESTService
    {
        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        public string HelloWorld(string helloWorld)
        {
            HefezopfServiceClient client = new HefezopfServiceClient();
            return client.HelloWorld(helloWorld);
        }

        /// <summary>
        /// Returns a hello world string from the database.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        public string HelloWorldFromDatabase(string helloWorld)
        {
            HefezopfServiceClient client = new HefezopfServiceClient();
            return client.HelloWorldFromDatabase(helloWorld);
        }
    }

    /// <summary>
    /// The REST Service Contract.
    /// </summary>
    [ServiceContract]
    public interface IHelloWorldRESTService
    {
        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        [OperationContract, WebGet(UriTemplate = "/HelloWorld/{helloWorld}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string HelloWorld(string helloWorld);

        /// <summary>
        /// Returns a hello world string from the database.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        [OperationContract, WebGet(UriTemplate = "/HelloWorldFromDatabase/{helloWorld}", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        string HelloWorldFromDatabase(string helloWorld);
    }

}
