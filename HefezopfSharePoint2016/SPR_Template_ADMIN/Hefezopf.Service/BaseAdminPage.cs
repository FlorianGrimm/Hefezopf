// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Microsoft.SharePoint.ApplicationPages;
    using Microsoft.SharePoint.Utilities;
    using Hefezopf.Service;
    using System.Net.Http;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using Microsoft.SharePoint.WebControls;

    /// <summary>
    /// Base class for administrative pages to inherit from.
    /// </summary>
    public class BaseAdminPage : System.Web.UI.Page
    {
        private SPWeb m_Web;

        /// <summary>
        /// The current service application to manage.
        /// </summary>
        private HefezopfServiceApplication serviceApplication;

        /// <summary>
        /// Gets the current service application to manage from the query string.
        /// </summary>
        internal HefezopfServiceApplication ServiceApplication
        {
            get
            {
                if (this.serviceApplication == null)
                {
                    string serviceApplicationId = this.Request.QueryString["id"];

                    if (string.IsNullOrEmpty(serviceApplicationId))
                    {
                        SPUtility.TransferToErrorPage(System.Web.HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ErrorNoServiceApplicationId", CultureInfo.CurrentCulture).ToString());
                    }

                    if (!IsValidGuid(serviceApplicationId))
                    {
                        SPUtility.TransferToErrorPage(System.Web.HttpContext.GetGlobalResourceObject("Hefezopf.Service.ServiceAdminResources", "ErrorInvalidServiceApplicationId", CultureInfo.CurrentCulture).ToString());
                    }

                    var service = HefezopfIisWebService.GetOrCreateService();
                    this.serviceApplication = service.Applications[new Guid(serviceApplicationId)] as HefezopfServiceApplication;
                }

                return this.serviceApplication;
            }
        }

        protected virtual bool AccessibleBySharePointAdminGroup => true;
        protected virtual bool AccessibleByDelegatedAdminGroup => true;
        public virtual string PageToRedirectOnOK => this.GetParentBucketURL();
        public virtual string PageToRedirectOnCancel => this.GetParentBucketURL();

        /// <summary>
        /// Gets the Required Page Parameters.
        /// </summary>
        protected virtual string[] RequiredPageParameters => new string[0];

        protected virtual string PageToRedirectOnMissingPageParameter { get { return null; } }

        protected bool IsPopUI => (!string.IsNullOrEmpty(this.Request.QueryString["IsDlg"]));

        public SPWeb Web
        {
            get
            {
                if (this.m_Web == null)
                {
                    this.m_Web = SPControl.GetContextWeb(this.Context);
                }
                return this.m_Web;
            }
        }

        /// <summary>
        /// Check if this is a Guid
        /// </summary>
        /// <param name="strGuid">the text to test.</param>
        /// <returns>true if it is a guid representation</returns>
        protected static bool IsValidGuid(string strGuid)
        {
            if (string.IsNullOrEmpty(strGuid))
            {
                return false;
            }
            bool result = false;
            try
            {
                new Guid(strGuid);
                result = true;
            }
            catch (FormatException)
            {
            }
            return result;
        }

        /// <summary>
        /// Validate the url
        /// </summary>
        /// <param name="strNextPage">the url to test</param>
        /// <returns>true if valid</returns>
        protected static bool IsValidNextPage(string strNextPage)
        {
            if (strNextPage == null)
            {
                return false;
            }
            int num = strNextPage.IndexOf("?");
            if (num < 0)
            {
                num = strNextPage.Length;
            }
            if (num == 0)
            {
                return false;
            }
            strNextPage = strNextPage.Substring(0, num);
            strNextPage = strNextPage.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            if (!strNextPage.EndsWith(".aspx"))
            {
                return false;
            }
            num = strNextPage.IndexOf(".aspx");
            if (num <= 0)
            {
                return false;
            }
            for (int i = 0; i < num; i++)
            {
                if (!char.IsLetterOrDigit(strNextPage, i) && strNextPage[i] != '/')
                {
                    return false;
                }
            }
            return true;
        }

        internal static void EnsureNTAuthentication(System.Web.HttpContext context)
        {
            if (!(context.User.Identity is System.Security.Principal.WindowsIdentity))
            {
                Send403(context);
            }
            var windowsIdentity = (System.Security.Principal.WindowsIdentity)context.User.Identity;
            if (windowsIdentity.IsAnonymous || !windowsIdentity.IsAuthenticated)
            {
                Send401(context);
            }
        }

        protected static void Send403(System.Web.HttpContext context)
        {
            SendResponse(context, 403, "403 FORBIDDEN", "text/plain");
        }

        protected static void Send401(System.Web.HttpContext context)
        {
            SendResponse(context, 401, "401 UNAUTHORIZED", "text/plain");
        }

        protected static void SendResponse(System.Web.HttpContext context, int code, string strBody, string strContentType)
        {
            var response = context.Response;
            object obj = context.Items["ResponseEnded"];
            if (obj != null && (bool)obj)
            {
                return;
            }
            context.Items["ResponseEnded"] = true;
            if (!string.IsNullOrEmpty(strContentType))
            {
                response.ContentType = strContentType;
            }
            context.ClearError();
            response.StatusCode = code;
            response.Clear();
            if (strBody != null)
            {
                response.Write(strBody);
            }
            response.End();
        }

        protected override void OnPreRender(EventArgs e)
        {
            string script = "var _fV4UI = true;";
            if (this.Web.UIVersion > 3 && this.Page != null)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(BaseAdminPage), "v4flag", script, true);
            }
            if (this.Page.Form != null)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, typeof(BaseAdminPage), "FormNameVariable", "var MSOWebPartPageFormName = '" + base.Form.Name + "';", true);
            }
            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string text = this.Context.Request.RawUrl.ToLowerInvariant();
            if (!text.StartsWith("/_admin/"))
            {
                SPUtility.HandleAccessDenied(new UnauthorizedAccessException(SPResource.GetString("AdminVs401Message", new object[0])));
            }
            EnsureNTAuthentication(this.Context);
            SPFarm local = SPFarm.Local;
            if (local == null)
            {
                throw new InvalidOperationException(SPResource.GetString("NoFarmObject", new object[0]));
            }
            bool flag = true;
            if (this.AccessibleBySharePointAdminGroup)
            {
                flag = !local.CurrentUserIsAdministrator();
                if (flag && this.AccessibleByDelegatedAdminGroup)
                {
                    SPAdministrationWebApplication local2 = SPAdministrationWebApplication.Local;
                    if (null != local2 && local2.CurrentUserIsDelegatedAdministrator())
                    {
                        flag = false;
                    }
                }
            }
            else
            {
                flag = this.Request.LogonUserIdentity.IsSystem;
            }
            if (flag)
            {
                SPUtility.HandleAccessDenied(new UnauthorizedAccessException(SPResource.GetString("AdminVs401Message", new object[0])));
            }
            string[] requiredPageParameters = this.RequiredPageParameters;
            for (int i = 0; i < requiredPageParameters.Length; i++)
            {
                if (string.IsNullOrEmpty((string)this.ViewState[requiredPageParameters[i]]))
                {
                    string value = base.Request.QueryString[requiredPageParameters[i]];
                    if (string.IsNullOrEmpty(value))
                    {
                        value = base.Request.Form[requiredPageParameters[i]];
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        if (this.PageToRedirectOnMissingPageParameter == null)
                        {
                            throw new SPException(SPResource.GetString("MissingRequiredQueryString", new object[]
                            {
                                requiredPageParameters[i]
                            }));
                        }
                        SPUtility.Redirect(this.PageToRedirectOnMissingPageParameter, SPRedirectFlags.Default, this.Context);
                    }
                    this.ViewState[requiredPageParameters[i]] = value;
                }
            }
            this.HideQuickLaunchHeaderAndFooter();
        }

        private void HideQuickLaunchHeaderAndFooter()
        {
            if (base.Master != null)
            {
                var contentPlaceHolder = base.Master.FindControl("PlaceHolderLeftNavBar") as System.Web.UI.WebControls.ContentPlaceHolder;
                if (contentPlaceHolder != null)
                {
                    var contentPlaceHolderInner = contentPlaceHolder.FindControl("PlaceHolderQuickLaunchTop") as System.Web.UI.WebControls.ContentPlaceHolder;
                    if (contentPlaceHolderInner != null)
                    {
                        contentPlaceHolderInner.Visible = false;
                    }
                    contentPlaceHolderInner = contentPlaceHolder.FindControl("PlaceHolderQuickLaunchBottom") as System.Web.UI.WebControls.ContentPlaceHolder;
                    if (contentPlaceHolderInner != null)
                    {
                        contentPlaceHolderInner.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// Generates a link to an administrative page.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <returns>A link to an administrative page.</returns>
        protected string GenerateLink(string pageName)
        {
            string template = "{0}?id={1}";

            return string.Format(CultureInfo.InvariantCulture, template, pageName, this.serviceApplication.Id);
        }

        /// <summary>
        /// Generates a link to a modal administrative page.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <returns>A link to an administrative page.</returns>
        protected string GeneratePopupLink(string pageName)
        {
            string template = "SP.UI.ModalDialog.showModalDialog({{url:'{0}?id={1}'}});return false;";

            return string.Format(CultureInfo.InvariantCulture, template, pageName, this.serviceApplication.Id);
        }

        protected string GetParentBucketURL()
        {
            string result = "/default.aspx";
            var siteMapProvider = System.Web.SiteMap.Providers["SPXmlAdminContentMapProvider"];
            if (siteMapProvider != null)
            {
                var siteMapNode = siteMapProvider.FindSiteMapNode(System.Web.HttpContext.Current);
                if (siteMapNode != null && siteMapNode.ParentNode != null)
                {
                    result = siteMapNode.ParentNode.Url;
                }
            }
            return result;
        }
    }
}