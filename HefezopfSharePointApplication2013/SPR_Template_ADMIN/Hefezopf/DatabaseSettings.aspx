<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ContentDatabaseSection" src="~/_admin/ContentDatabaseSection.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DatabaseSettings.aspx.cs" Inherits="Hefezopf.SharePoint.Application.Administration.DatabaseSettingsPage" MasterPageFile="~/_admin/admin.master" %>
<asp:Content ID="contentPageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="contentPageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <asp:Literal ID="literalDatabaseSettingsPageTitle" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, DatabaseSettingsPageTitle %>'></asp:Literal>
</asp:Content>
<asp:Content ID="contentTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <a href="/_admin/ServiceApplications.aspx"><SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server" Text="<%$Resources:spadmin, svcappsadm_pagetitle%>" EncodeMethod='HtmlEncode'/></a> : 
    <a href="/_admin/Hefezopf/ManageApplication.aspx?id=<%= Request.QueryString["id"] %>"><asp:Literal ID="literalManagePage" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, ApplicationManagementPageTitle %>'></asp:Literal></a> :
    <asp:Literal ID="literalPageTitleInTitleArea" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, DatabaseSettingsPageTitleInTitleArea %>'></asp:Literal>
</asp:Content>
<asp:content ID="contentPageDescription" ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
	<asp:Literal id="literalPageDescription" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, DatabaseSettingsPageDescription %>' Mode="PassThrough" />
</asp:content>
<asp:Content ID="contentMain" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Panel ID="panelError" runat="server" Visible="false">
        <asp:Label ID="labelError" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="panelMain" runat="server">
    <table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0"> <tr> <td class="ms-descriptionText"> <asp:Label ID="LabelMessage" Runat="server" EnableViewState="False" CssClass="ms-descriptionText"/> </td> </tr> <tr> <td class="ms-error"><asp:Label ID="LabelErrorMessage" runat="server" EnableViewState="False" /></td> </tr> <tr> <td class="ms-descriptionText"> <asp:ValidationSummary ID="validationSummary" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"> </asp:ValidationSummary> </td> </tr> </table>
    <table border="0" cellspacing="0" cellpadding="0" width="100%" class="ms-propertysheet">
        <wssuc:InputFormSection Title="" ID="buttonSectionTop" runat="server">
            <template_inputformcontrols>
                <wssuc:InputFormControl LabelText="" runat="server">
                    <Template_control>
				        <asp:Button runat="server" CssClass="ms-ButtonHeightWidth" OnClick="ButtonOk_Click" Text="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, ButtonOKText %>" ID="buttonOkTop" />
				        <asp:Button runat="server" CssClass="ms-ButtonHeightWidth" OnClick="ButtonCancel_Click" Text="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, ButtonCancelText %>" ID="buttonCancelTop" />
                    </Template_control>
                </wssuc:InputFormControl>
            </template_inputformcontrols>
        </wssuc:InputFormSection>

        <wssuc:ContentDatabaseSection ID="databaseSection" 
                Title="<%$Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, DatabaseSettingsDatabaseSectionTitle %>" 
                IncludeSearchServer="false" 
                Collapsible="true"
                OnDatabaseSubmitted="ValidateDatabase"
                runat="server" ></wssuc:ContentDatabaseSection>

        <wssuc:InputFormSection Title="" ID="buttonSectionBottom" runat="server">
            <template_inputformcontrols>
                <wssuc:InputFormControl LabelText="" runat="server">
                    <Template_control>
				    <asp:Button runat="server" CssClass="ms-ButtonHeightWidth" OnClick="ButtonOk_Click" Text="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, ButtonOKText %>" ID="buttonOkBottom" />&nbsp;&nbsp;
				    <asp:Button runat="server" CssClass="ms-ButtonHeightWidth" OnClick="ButtonCancel_Click" Text="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, ButtonCancelText %>" ID="buttonCancelBottom" />
                    </Template_control>
                </wssuc:InputFormControl>
            </template_inputformcontrols>
        </wssuc:InputFormSection>

    </table>
    </asp:Panel>
</asp:Content>


