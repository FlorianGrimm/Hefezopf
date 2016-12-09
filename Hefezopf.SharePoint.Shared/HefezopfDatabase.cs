// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Principal;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;

    /// <summary>
    /// Custom Database class. Contains logic to Provision/Upgrade/Remove the custom database.
    /// </summary>
    [System.Runtime.InteropServices.Guid("762262ec-e445-4c03-8919-b7a95671d44c")]
    public sealed class HefezopfDatabase : SPDatabase
    {
        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfDatabase"/> class. Default constructor (required for SPPersistedObject serialization). Do not call this directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HefezopfDatabase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HefezopfDatabase"/> class. Use this constructor when creating a new database in the farm.
        /// </summary>
        /// <param name="databaseParameters">The database parameters to use when creating the new database.</param>
        public HefezopfDatabase(SPDatabaseParameters databaseParameters)
            : base(databaseParameters)
        {
            this.Status = SPObjectStatus.Disabled;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Contains the logic to Provision the database.
        /// </summary>
        public override void Provision()
        {
            // Stop if the DB has already been provisioned
            if (this.Status == SPObjectStatus.Online)
            {
                return;
            }

            // Update the status to provisioning
            this.Status = SPObjectStatus.Provisioning;
            this.Update();

            // Run the provisioning scripts if we are not attaching to an existing database
            if (!this.Exists)
            {
                // Create any DB options
                Dictionary<DatabaseOptions, bool> options = new Dictionary<DatabaseOptions, bool>();
                options.Add(SPDatabase.DatabaseOptions.AutoClose, false);
                using (System.IO.TextReader textReader = new System.IO.StringReader("SET NOCOUNT ON;"))
                {
                    var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(this.DatabaseConnectionString);
                    SPDatabase.Provision(csb, textReader, options);
                }
            }

            this.Status = SPObjectStatus.Online;
            this.Update();
        }

        /// <summary>
        /// Grants the application pool service account rights to the database. Call this after you call Provision()
        /// on this class.
        /// </summary>
        /// <param name="processSecurityIdentifier">The application pool service account.</param>
        public void GrantApplicationPoolAccess(SecurityIdentifier processSecurityIdentifier)
        {
            this.GrantAccess(processSecurityIdentifier, "db_owner");
        }

        #endregion
    }
}
