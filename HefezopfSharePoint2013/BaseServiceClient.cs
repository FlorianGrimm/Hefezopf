// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;

    /// <summary>
    /// Base class for all service clients to inherit from.
    /// </summary>
    public abstract class BaseServiceClient
    {
        #region Fields

        /// <summary>
        /// A locking object for caching a channel factory.
        /// </summary>
        private static object channelFactoryLock = new object();

        /// <summary>
        /// Dictionary to hold cached factories
        /// </summary>
        private static Dictionary<string, object> factoryDictionary = new Dictionary<string, object>();

        /// <summary>
        /// The current service context.
        /// </summary>
        private SPServiceContext currentServiceContext;

        /// <summary>
        /// The current service application proxy.
        /// </summary>
        private HefezopfServiceApplicationProxy currentProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseServiceClient"/> class.
        /// </summary>
        protected BaseServiceClient()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseServiceClient"/> class.
        /// </summary>
        /// <param name="serviceContext">A <see cref="Microsoft.SharePoint.SPServiceContext"/> instance.</param>
        protected BaseServiceClient(SPServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                if (SPServiceContext.Current != null)
                {
                    this.currentServiceContext = SPServiceContext.Current;
                }
                else
                {
                    this.currentServiceContext = SPServiceContext.GetContext(SPServiceApplicationProxyGroup.Default, new SPSiteSubscriptionIdentifier(Guid.Empty));
                }
            }
            else
            {
                this.currentServiceContext = serviceContext;
            }

            if (this.currentServiceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate method that gets executed in the service application tier.
        /// </summary>
        /// <typeparam name="TChannel">The type of Channel.</typeparam>
        /// <param name="channel">The current channel to execute across.</param>
        protected delegate void CodeToExecuteOnChannel<TChannel>(TChannel channel);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current Service Application Proxy
        /// </summary>
        internal HefezopfServiceApplicationProxy Proxy
        {
            get
            {
                if (this.currentProxy == null)
                {
                    this.currentProxy = (HefezopfServiceApplicationProxy)this.currentServiceContext.GetDefaultProxy(typeof(HefezopfServiceApplicationProxy));
                }

                return this.currentProxy;
            }
        }

        /// <summary>
        /// Gets the name and extension of the .svc service file.
        /// </summary>
        protected abstract string EndPoint { get; }

        #endregion

        #region Methods

        /// <summary>
        /// This is the method that will execute the WCF service operation across the WCF channel.
        /// </summary>
        /// <param name="codeToExecute">A delegate, pointing to the method on the Service Contract interface that you want to run.</param>
        /// <param name="asProcess">Whether to run as the passed-through identity of the user (supported in claims mode), or the app pool account.</param>
        /// <typeparam name="TChannel">The type of channel.</typeparam>
        /// <remarks>
        /// This is a good place to use an SPMonitoredScope so that you can
        /// monitor execution times of your service calls.
        /// This method sets up the load balancer, and calls into the service application.
        /// If an exception occurs  during the load balancer operation, make sure to report it as a failure if it is due to a
        /// communication issue. Otherwise, if it is an exception in your application logic or execution, do not report the failure
        /// to ensure that the server is not taken out of the load balancer.
        /// </remarks>
        protected void ExecuteOnChannel<TChannel>(CodeToExecuteOnChannel<TChannel> codeToExecute, bool asProcess)
        {
            if (codeToExecute == null)
            {
                throw new ArgumentNullException("codeToExecute");
            }

            if (this.Proxy == null)
            {
                throw new EndpointNotFoundException("Could not find an endpoint because the proxy was not found.");
            }

            using (new SPMonitoredScope(string.Format(CultureInfo.InvariantCulture, "{0} - {1}", this.EndPoint, codeToExecute.Method.Name)))
            {
                SPServiceLoadBalancer loadBalancer = this.Proxy.LoadBalancer;

                if (loadBalancer == null)
                {
                    throw new InvalidOperationException("No Load Balancer was available.");
                }

                SPServiceLoadBalancerContext loadBalancerContext = loadBalancer.BeginOperation();
                try
                {
                    using (new SPServiceContextScope(this.currentServiceContext))
                    {
                        ICommunicationObject channel = (ICommunicationObject)this.GetChannel<TChannel>(loadBalancerContext, asProcess);
                        try
                        {
                            codeToExecute((TChannel)channel);
                            channel.Close();
                        }
                        finally
                        {
                            if (channel.State != CommunicationState.Closed)
                            {
                                channel.Abort();
                            }
                        }
                    }
                }
                catch (TimeoutException)
                {
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    throw;
                }
                catch (EndpointNotFoundException)
                {
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    throw;
                }
                catch (ServerTooBusyException)
                {
                    loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    throw;
                }
                catch (CommunicationException exception)
                {
                    if (!(exception is FaultException))
                    {
                        loadBalancerContext.Status = SPServiceLoadBalancerStatus.Failed;
                    }

                    throw;
                }
                finally
                {
                    loadBalancerContext.EndOperation();
                }
            }
        }

        /// <summary>
        /// Gets the endpoint configuration name for a given endpoint address.
        /// </summary>
        /// <param name="address">Endpoint address.</param>
        /// <returns>The endpoint configuration name.</returns>
        /// <remarks>The returned endpoint must match one of the endpoint element names in the client.config file.</remarks>
        private static string GetEndpointConfigurationName(Uri address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (address.Scheme == Uri.UriSchemeNetTcp)
            {
                if (address.AbsolutePath.EndsWith("/secure", StringComparison.OrdinalIgnoreCase))
                {
                    return "tcp-ssl";
                }
                else
                {
                    return "tcp";
                }
            }
            else if (address.Scheme == Uri.UriSchemeHttps)
            {
                return "https";
            }
            else if (address.Scheme == Uri.UriSchemeHttp)
            {
                return "http";
            }
            else
            {
                throw new NotSupportedException("Unsupported endpoint address.");
            }
        }

        /// <summary>
        /// This method gets the end point address to the WCF service. This is where you can support multiple .svc files per service application,
        /// by looking for the default end point name defined in the ServiceApplication class, and swapping it for the EndPoint property that you
        /// set in your Service Client.
        /// </summary>
        /// <param name="loadBalancerContext">The LoadBalancer context obtained from the Service Application proxy</param>
        /// <param name="asProcess">Whether to run as the user or the app pool account.</param>
        /// <typeparam name="TChannel">The type of channel.</typeparam>
        /// <returns>A WCF Communication channel to execute across</returns>
        private TChannel GetChannel<TChannel>(SPServiceLoadBalancerContext loadBalancerContext, bool asProcess)
        {
            TChannel channel = default(TChannel);

            EndpointAddress endPointAddress = new EndpointAddress(new Uri(loadBalancerContext.EndpointAddress.AbsoluteUri.Replace("ReplaceableEndPointName.svc", this.EndPoint)), new AddressHeader[0]);

            string endPointConfigurationName = BaseServiceClient.GetEndpointConfigurationName(endPointAddress.Uri);

            ChannelFactory<TChannel> channelFactory = this.GetChannelFactory<TChannel>(endPointConfigurationName, this.EndPoint);

            if (!asProcess)
            {
                channel = channelFactory.CreateChannelActingAsLoggedOnUser<TChannel>(endPointAddress);
            }
            else
            {
                channel = channelFactory.CreateChannelAsProcess<TChannel>(endPointAddress);
            }

            ((ICommunicationObject)channel).Open();
            return channel;
        }

        /// <summary>
        /// Gets the channel factory necessary for acquiring a channel.
        /// </summary>
        /// <param name="endPointConfigurationName">The end point configuration name, which will typically be TCP, HTTPS, HTTP, etc.</param>
        /// <param name="endPointName">Name of end point, used as key for dictionary to cached channel factories.</param>
        /// <typeparam name="TChannel">The type of channel.</typeparam>
        /// <returns>A <see cref="ChannelFactory" /> class.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Channel factory is cached for the lifetime of the app domain,and does not require disposal.")]
        private ChannelFactory<TChannel> GetChannelFactory<TChannel>(string endPointConfigurationName, string endPointName)
        {
            ChannelFactory<TChannel> currentFactory = null;

            if (factoryDictionary.ContainsKey(endPointName))
            {
                currentFactory = factoryDictionary[endPointName] as ChannelFactory<TChannel>;
            }

            if (currentFactory == null)
            {
                lock (channelFactoryLock)
                {
                    // Make sure that a channel factory wasn't created during the time it took us to get a lock
                    if (factoryDictionary.ContainsKey(endPointName))
                    {
                        currentFactory = factoryDictionary[endPointName] as ChannelFactory<TChannel>;
                    }

                    if (currentFactory == null)
                    {
                        // Create a channel factory to be cached
                        currentFactory = new ConfigurationChannelFactory<TChannel>(
                            endPointConfigurationName,
                            this.Proxy.Configuration,
                            null);

                        // Configure the channel factory for claims-based authentication
                        ((ChannelFactory<TChannel>)currentFactory).ConfigureCredentials(SPServiceAuthenticationMode.Claims);
                        factoryDictionary[endPointName] = currentFactory;
                    }
                }
            }

            return currentFactory;
        }
        #endregion
    }
}
