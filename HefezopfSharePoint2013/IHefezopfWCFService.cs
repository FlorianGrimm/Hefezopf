//-----------------------------------------------------------------------
// <copyright file="IHelloWorldWCFService.cs" company="">
// Copyright © 
// </copyright>
//-----------------------------------------------------------------------

namespace Hefezopf.Service
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// The Service Contract.
    /// </summary>
    [ServiceContract]
    [System.Runtime.InteropServices.Guid("79f521b0-e048-479e-8c54-7a93b636e6a4")]
    internal interface IHelloWorldWCFService
    {
        #region Methods

        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        [OperationContract]
        string HelloWorld(string helloWorld);

        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        [OperationContract]
        string HelloWorldFromDatabase(string helloWorld);

        #endregion
    }
}
