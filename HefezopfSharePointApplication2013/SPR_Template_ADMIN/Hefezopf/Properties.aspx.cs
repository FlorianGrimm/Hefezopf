//-----------------------------------------------------------------------
// <copyright file="Properties.aspx.cs" company="">
// Copyright © 
// </copyright>
//-----------------------------------------------------------------------

namespace Hefezopf.SharePoint.Application.Administration
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
    using Hefezopf.SharePoint.Application.Administration;
    using Service;

    /// <summary>
    /// The Application Properties Page.
    /// </summary>
    public partial class PropertiesPage : BaseAdminPage
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// The required query string parameters
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
                SPUtility.TransferToErrorPage(HttpContext.GetGlobalResourceObject("Hefezopf.SharePoint.Application.ServiceAdminResources", "ErrorNoServiceApplication", CultureInfo.CurrentCulture).ToString());
            }

            ((DialogMaster)Page.Master).OkButton.Click += new EventHandler(this.OkButton_Click);

            if (!this.IsPostBack)
            {
                (this.applicationPoolSection as IisWebServiceApplicationPoolSection).SetSelectedApplicationPool(this.ServiceApplication.ApplicationPool);
                this.textBoxServiceName.Text = this.ServiceApplication.Name;
            }

            // set masterpage's ok button as submit for form
            Form.DefaultButton = ((DialogMaster)Master).OkButton.UniqueID;
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Click event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void OkButton_Click(object sender, EventArgs e)
        {
            this.Validate();

            if (this.IsValid)
            {
                using (SPLongOperation operation = new SPLongOperation(this))
                {
                    operation.Begin();
                    this.ServiceApplication.Name = this.textBoxServiceName.Text.Trim();
                    IisWebServiceApplicationPoolSection iisSection = this.applicationPoolSection as IisWebServiceApplicationPoolSection;
                    this.ServiceApplication.ApplicationPool = iisSection.GetOrCreateApplicationPool();
                    this.ServiceApplication.Update();
                    operation.EndScript("window.frameElement.commitPopup();");
                }
            }
        }

        /// <summary>
        /// ServerValidate event.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void CustomValidatorServiceNameDuplicate_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            string serviceName = e.Value;

            if (string.Equals(this.ServiceApplication.Name, serviceName, StringComparison.CurrentCultureIgnoreCase))
            {
                e.IsValid = true;
                return;
            }

            // Get the service
            HefezopfService service = HefezopfService.GetOrCreateService();

            // Try to get a duplicate service application
            HefezopfServiceApplication duplicate = SPFarm.Local.GetObject(serviceName, service.Id, typeof(HefezopfServiceApplication)) as HefezopfServiceApplication;

            if (duplicate != null)
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }
        }

        #endregion
    }
}
