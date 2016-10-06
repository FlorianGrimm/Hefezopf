// Hefezopf
// MIT License
// Copyright (c) 2016 Florian GRimm

namespace Hefezopf.SharePoint
{
#pragma warning disable SA1201 // Elements must appear in the correct order

    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using Hefezopf.Service;
    using System.ServiceModel.Web;
    //using Microsoft.SharePoint.Administration;

    /// <summary>
    /// The REST Service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [System.Runtime.InteropServices.Guid("a42247d1-91e4-40f6-9653-0daa6667762a")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by the WCF runtime automatically.")]
    public class HefezopfRestService : IHefezopfRESTService
    {
        ///// <summary>
        ///// Returns a hello world string.
        ///// </summary>
        ///// <param name="helloWorld">An input string of text.</param>
        ///// <returns>A string of text echoing the input value.</returns>
        //public string HelloWorld(string helloWorld)
        //{
        //    HefezopfServiceClient client = new HefezopfServiceClient();
        //    return client.HelloWorld(helloWorld);
        //}

        ///// <summary>
        ///// Returns a hello world string from the database.
        ///// </summary>
        ///// <param name="helloWorld">An input string of text.</param>
        ///// <returns>A string of text echoing the input value.</returns>
        //public string HelloWorldFromDatabase(string helloWorld)
        //{
        //    HefezopfServiceClient client = new HefezopfServiceClient();
        //    return client.HelloWorldFromDatabase(helloWorld);
        //}

        public string Execute(string request)
        {
            HefezopfServiceClient client = new HefezopfServiceClient();
            return client.Execute(request);
        }

        public string[] ExecuteMany(string[] requests)
        {
            HefezopfServiceClient client = new HefezopfServiceClient();
            return client.ExecuteMany(requests);
        }

        public string ExecuteQueue(string request)
        {
            HefezopfServiceClient client = new HefezopfServiceClient();
            return client.ExecuteQueue(request);
        }
    }
}
