// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.Administration
{
    using System;
    using System.Globalization;
    using System.Web;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.Utilities;
    using Microsoft.SharePoint.WebControls;
    using Hefezopf.Service.Administration;

    /// <summary>
    /// The Manage Application Page.
    /// </summary>
    public partial class ManageApplicationPage : BaseAdminPage
    {
        #region Fields

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

            // Bind the page so we can setup all our hyperlinks
            this.DataBind();
        }

        #endregion

        #region Control Events

        #endregion

        #region Methods

        #endregion
    }
}
