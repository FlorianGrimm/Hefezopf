// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Contracts.Communication;
    using Microsoft.SharePoint;

    /// <summary>
    /// This is the class that is accessible to the Client callers (web parts, user controls, timer jobs, etc.).
    /// </summary>
    public class HefezopfServiceClient
        : BaseServiceClient
        , Hefezopf.Contracts.Communication.IHZTransportContract
        , Hefezopf.Contracts.Communication.IHZServiceContract
    {
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

        public string Execute(string request)
        {
            string response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.Execute(request);
                },
                false);

            return response;
        }

        public string[] ExecuteMany(string[] requests)
        {
            string[] response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.ExecuteMany(requests);
                },
                false);

            return response;
        }

        public string ExecuteQueue(string request)
        {
            string response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.ExecuteQueue(request);
                },
                false);

            return response;
        }

        public HZServiceResponce ExecuteOneAction(HZServiceRequest request)
        {
            HZServiceResponce response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.ExecuteOneAction(request);
                },
                false);

            return response;
        }

        public HZServiceResponce[] ExecuteManyActions(HZServiceRequest[] requests)
        {
            HZServiceResponce[] response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.ExecuteManyActions(requests);
                },
                false);

            return response;
        }

        public HZServiceResponce ExecuteOneActionQueued(HZServiceRequest request)
        {
            HZServiceResponce response = null;

            this.ExecuteOnChannel<IHefezopfWCFService>(
                delegate (IHefezopfWCFService channel)
                {
                    response = channel.ExecuteOneActionQueued(request);
                },
                false);

            return response;
        }
    }
}