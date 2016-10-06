//-----------------------------------------------------------------------
// <copyright file="DatabaseSettings.aspx.cs" company="">
// Copyright © 
// </copyright>
//-----------------------------------------------------------------------
namespace Hefezopf.Service.Administration
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.UI.WebControls;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.ApplicationPages;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.SharePoint.WebControls;
    using Hefezopf.Service;

    /// <summary>
    /// The Database Settings Page
    /// </summary>
    public partial class DatabaseSettingsPage : BaseAdminPage
    {
        #region Fields

        /// <summary>
        /// The current database parameters.
        /// </summary>
        private SPDatabaseParameters databaseParameters;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the required query string parameters.
        /// </summary>
        protected override string[] RequiredPageParameters
        {
            get
            {
                return new string[] { "id" };
            }
        }

        #endregion

        #region Page Events

        /// <summary>
        /// Page_Load event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.ServiceApplication == null)
            {
                SPUtility.TransferToErrorPage(HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ErrorNoServiceApplication", CultureInfo.CurrentCulture).ToString());
            }

            ContentDatabaseSection databaseSectionControl = this.databaseSection as ContentDatabaseSection;

            if (!Page.IsPostBack)
            {
                if (this.ServiceApplication.Database == null)
                {
                    // Set some default values
                    databaseSectionControl.DatabaseName = "HefezopfService_DB";
                    databaseSectionControl.Visible = true;
                }
                else
                {
                    databaseSectionControl.Visible = true;
                    databaseSectionControl.DisplayMode = ContentDatabaseSectionMode.AuthenticationEdit;
                    databaseSectionControl.ConnectionString = this.ServiceApplication.Database.DatabaseConnectionString;
                }
            }
        }

        /// <summary>
        /// Page_Error event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
            }
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ButtonOk_Click(object sender, EventArgs e)
        {
            // Validate
            this.Validate();

            if (!this.IsValid)
            {
                return;
            }

            ContentDatabaseSection databaseSectionControl = this.databaseSection as ContentDatabaseSection;

            // We are valid
            // Register the database
            using (SPLongOperation operation = new SPLongOperation(this))
            {
                operation.LeadingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "DatabaseSettingsCreateOperationLeadingHtml", CultureInfo.CurrentCulture).ToString();
                operation.TrailingHTML = HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "DatabaseSettingsCreateOperationTrailingHtml", CultureInfo.CurrentCulture).ToString();
                operation.Begin();

                if (databaseSectionControl.DisplayMode == ContentDatabaseSectionMode.AuthenticationEdit)
                {
                    // We are only changing credentials here, do not reprovision.
                    if (string.Equals(this.ServiceApplication.Database.DatabaseConnectionString, databaseSectionControl.ConnectionString, StringComparison.OrdinalIgnoreCase))
                    {
                        // No change made, exit the application.
                        operation.End(string.Format(CultureInfo.InvariantCulture, "/_admin/Hefezopf.Service/ManageApplication.aspx?id={0}", SPHttpUtility.UrlKeyValueEncode(this.ServiceApplication.Id)));
                    }
                    else
                    {
                        if (databaseSectionControl.UseWindowsAuthentication)
                        {
                            // Switching to windows authentication.
                            this.ServiceApplication.Database.Username = string.Empty;
                            this.ServiceApplication.Database.Password = string.Empty;
                        }
                        else
                        {
                            this.ServiceApplication.Database.Username = databaseSectionControl.DatabaseUserName;
                            this.ServiceApplication.Database.Password = databaseSectionControl.DatabasePassword;
                        }

                        this.ServiceApplication.Database.GrantOwnerAccessToDatabaseAccount();
                    }

                    // Set the failover instance
                    this.ServiceApplication.Database.AddFailoverServiceInstance(databaseSectionControl.FailoverDatabaseServer);

                    this.ServiceApplication.Database.Update();
                    operation.End(string.Format(CultureInfo.InvariantCulture, "/_admin/Hefezopf.Service/ManageApplication.aspx?id={0}", SPHttpUtility.UrlKeyValueEncode(this.ServiceApplication.Id)));
                }

                // Create the database
                HefezopfDatabase database = new HefezopfDatabase(this.databaseParameters);

                // Provision the database (runs the Create scripts)
                database.Provision();

                // Grant the database the proper permissions
                database.GrantApplicationPoolAccess(this.ServiceApplication.ApplicationPool.ProcessAccount.SecurityIdentifier);

                // Add the failover server instance (the base class does not do this for you)
                if (!string.IsNullOrEmpty(this.databaseParameters.FailoverPartner))
                {
                    database.AddFailoverServiceInstance(this.databaseParameters.FailoverPartner);
                }

                // Establish a relationship between the service application and the database
                this.ServiceApplication.Database = database;
                this.ServiceApplication.Update();

                operation.End(string.Format(CultureInfo.InvariantCulture, "/_admin/Hefezopf.Service/ManageApplication.aspx?id={0}", SPHttpUtility.UrlKeyValueEncode(this.ServiceApplication.Id)));
            }
        }

        /// <summary>
        /// ValidateDatabase event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ValidateDatabase(object sender, ServerValidateEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            ContentDatabaseSection databaseSection = this.databaseSection as ContentDatabaseSection;

            this.databaseParameters = SPDatabaseParameters.CreateParameters(
                databaseSection.DatabaseName,
                databaseSection.DatabaseServer,
                databaseSection.UseWindowsAuthentication ? null : databaseSection.DatabaseUserName,
                databaseSection.UseWindowsAuthentication ? null : databaseSection.DatabasePassword,
                databaseSection.FailoverDatabaseServer,
                SPDatabaseParameterOptions.None);

            // Validate the database - There are several options:
            // SPDatabaseValidation.None = Don't do any validation. Useful if you want to allow a user to specify a database that already exists.
            // SPDatabaseValidation.CreateNew = Throws an exception if a database with the same name already exists. Useful if you don't want to allow specifiying an existing database.
            // SPDatabaseValidation.AttachExisting = Throws an exception if a database with the given name does not exist. Useful on screens where you edit the db credentials and want to ensure the db exists first.
            if (databaseSection.DisplayMode == ContentDatabaseSectionMode.Default)
            {
                this.databaseParameters.Validate(SPDatabaseValidation.None);
            }
            else
            {
                this.databaseParameters.Validate(SPDatabaseValidation.AttachExisting);
            }

            // If the code did not throw above, then everything should be valid.
            e.IsValid = true;
        }

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(CultureInfo.InvariantCulture, "ManageApplication.aspx?id={0}", SPHttpUtility.UrlKeyValueEncode(this.ServiceApplication.Id)));
        }

        #endregion

        #region Methods

        #endregion
    }
}
