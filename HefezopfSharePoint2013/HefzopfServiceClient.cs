//-----------------------------------------------------------------------
// <copyright file="HelloWorldServiceClient.cs" company="">
// Copyright ©
// </copyright>
//-----------------------------------------------------------------------

namespace Hefezopf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.SharePoint;

    /// <summary>
    /// This is the class that is accessible to the Client callers (web parts, user controls, timer jobs, etc.).
    /// </summary>
    public class HefezopfServiceClient : BaseServiceClient
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceClient"/> class.
        /// </summary>
        public HefezopfServiceClient()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfServiceClient"/> class.
        /// </summary>
        /// <param name="serviceContext">An <see cref="Microsoft.SharePoint.Administration.SPServiceContext" />.</param>
        public HefezopfServiceClient(SPServiceContext serviceContext)
            : base(serviceContext)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the actual (.svc file) for this service.
        /// </summary>
        /// <remarks>
        /// Service applications are designed to support a single endpoint .svc file. For more complicated
        /// service applications with many different types of services, it makes sense to create several .svc files
        /// and classes. To support multiple end points, use a recognizable string here, and swap it out dynamically
        /// in the BaseServiceClient's GetEndPoint method after the load balancer has provided the full path to this
        /// original end point.
        /// </remarks>
        protected override string EndPoint
        {
            get { return "Hefezopf.svc"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the HelloWorld method on the service through the proxy.
        /// </summary>
        /// <param name="inputText">Input text.</param>
        /// <returns>A string of text echoing the input, with the current date time appended.</returns>
        public string HelloWorld(string inputText)
        {
            string response = null;

            this.ExecuteOnChannel<IHelloWorldWCFService>(
                delegate(IHelloWorldWCFService channel)
                {
                    response = channel.HelloWorld(inputText);
                },
                false);

            return response;
        }

        /// <summary>
        /// Executes the HelloWorld method on the service through the proxy.
        /// </summary>
        /// <param name="inputText">Input text.</param>
        /// <returns>A string of text echoing the input, with the current date time appended.</returns>
        public string HelloWorldFromDatabase(string inputText)
        {
            string response = null;

            this.ExecuteOnChannel<IHelloWorldWCFService>(
                delegate(IHelloWorldWCFService channel)
                {
                    response = channel.HelloWorldFromDatabase(inputText);
                },
                false);

            return response;
        }

        #endregion
    }
}