// Hefezopf
// MIT License
// Copyright (c) 2016 Florian GRimm

namespace Hefezopf.Service
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;
    using Contracts.Communication;
    using Microsoft.SharePoint.Administration;

    /// <summary>
    /// The WCF Service.
    /// </summary>
    [System.Runtime.InteropServices.Guid("a43db183-e3a5-4e98-9f63-ad4d69f327f6")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by the WCF runtime automatically.")]
    public class HefezopfWCFService : IHefezopfWCFService
    {
#if no
        #region Methods

        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        public string HelloWorld(string helloWorld)
        {
            return "Hello World - You entered: " + helloWorld;
        }

        /// <summary>
        /// Returns a hello world string.
        /// </summary>
        /// <param name="helloWorld">An input string of text.</param>
        /// <returns>A string of text echoing the input value.</returns>
        public string HelloWorldFromDatabase(string helloWorld)
        {
            HefezopfServiceApplication application = SPIisWebServiceApplication.Current as HefezopfServiceApplication;
            if (application == null)
            {
                throw new InvalidOperationException("Could not find the current Service Application.");
            }

            using (SqlConnection connection = new SqlConnection(application.Database.DatabaseConnectionString))
            {
                using (SqlCommand command = new SqlCommand("HelloWorld", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@input", helloWorld);
                    SqlParameter output = new SqlParameter("@output", SqlDbType.NVarChar) { Direction = ParameterDirection.Output, Size = -1 };
                    command.Parameters.Add(output);

                    connection.Open();

                    command.ExecuteNonQuery();

                    string returnValue = (string)command.Parameters[1].Value;

                    return returnValue;
                }
            }
        }

        #endregion
#endif
        public string Execute(string request)
        {
            return request;
        }

        public string[] ExecuteMany(string[] requests)
        {
            return requests;
        }

        public string ExecuteQueue(string request)
        {
            return null;
        }

        public HZServiceResponce[] ExecuteManyActions(HZServiceRequest[] requests)
        {
            return null;
        }

        public HZServiceResponce ExecuteOneAction(HZServiceRequest request)
        {
            return null;
        }

        public HZServiceResponce ExecuteOneActionQueued(HZServiceRequest request)
        {
            return null;
        }
    }
}
