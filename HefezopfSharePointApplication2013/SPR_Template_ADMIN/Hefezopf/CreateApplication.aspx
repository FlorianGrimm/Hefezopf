﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>  
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="IisWebServiceApplicationPoolSection" src="~/_admin/IisWebServiceApplicationPoolSection.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateApplication.aspx.cs" Inherits="Hefezopf.SharePoint.Application.Administration.CreateApplicationPage" MasterPageFile="~/_layouts/15/dialog.master" %>
<asp:Content ID="contentDialogHeaderPageTitle" ContentPlaceHolderID="PlaceHolderDialogHeaderPageTitle" runat="server">
    <asp:Literal ID="literalDialogHeaderPageTitle" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreatePageTitle %>'></asp:Literal>
</asp:Content>
<asp:Content ID="contentDialogDescription" ContentPlaceHolderID="PlaceHolderDialogDescription" runat="server">
    <asp:Literal id="literalDialogDescription" runat="server" Text='<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreatePageDescription %>' Mode="PassThrough" />
</asp:Content>
<asp:Content ID="contentHelpLink" ContentPlaceHolderID="PlaceHolderHelpLink" runat="server">
</asp:Content>
<asp:Content ID="contentDialogImage" ContentPlaceHolderID="PlaceHolderDialogImage" runat="server">
    <img src="/_admin/Hefezopf/ca-icon.png" alt="" />
</asp:Content>
<asp:Content ID="contentDialogBodyMainSection" contentplaceholderid="PlaceHolderDialogBodyMainSection" runat="server">
    <asp:Panel ID="panelError" runat="server" Visible="false">
        <asp:Label ID="labelError" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
    </asp:Panel>
    <table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0"> <tr> <td class="ms-descriptionText"> <asp:Label ID="labelMessage" runat="server" EnableViewState="False" CssClass="ms-descriptionText"/> </td> </tr> <tr> <td class="ms-error"><asp:Label ID="labelErrorMessage" runat="server" EnableViewState="False" /></td> </tr> <tr> <td class="ms-descriptionText"> <asp:ValidationSummary ID="validationSummary" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"> </asp:ValidationSummary> </td> </tr> </table>
    <table border="0" cellspacing="0" cellpadding="0" width="100%" class="ms-propertysheet">
            <wssuc:InputFormSection runat="server" Title="<%$Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationNameSectionTitle%>" Description="<%$Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationNameSectionDescription%>">
                <Template_InputFormControls>
                    <wssuc:InputFormControl runat="server" LabelText="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationNameLabel %>" >
                        <Template_Control>
                            <asp:TextBox runat="server" ID="textBoxServiceName" MaxLength="50" Width="300px" />
    			            <asp:RequiredFieldValidator ID="requiredFieldValidatorServiceName" runat="server" ControlToValidate="textBoxServiceName" Font-Bold="true" Forecolor="Red" Text="*" Display="Dynamic" ErrorMessage="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationNameRequiredError %>"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="customValidatorServiceNameDuplicate" runat="server" ControlToValidate="textBoxServiceName" OnServerValidate="CustomValidatorServiceNameDuplicate_ServerValidate" Font-Bold="true" Forecolor="Red" Text="*" Display="Dynamic" ErrorMessage="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationNameDuplicateError %>"></asp:CustomValidator>
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>      
            </wssuc:InputFormSection>
            <wssuc:IisWebServiceApplicationPoolSection runat="server" ID="applicationPoolSection" />
            <wssuc:InputFormSection runat="server" Title="<%$Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationDefaultProxySectionTitle%>" Description="<%$Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationDefaultProxySectionDescription%>">
                <Template_InputFormControls>
                    <wssuc:InputFormControl runat="server" LabelText="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationDefaultProxyLabel %>" >
                        <Template_Control>
                            <asp:CheckBox ID="checkBoxIncludeInDefaultProxy" runat="server" Text="<%$ Resources: Hefezopf.SharePoint.Application.ServiceAdminResources, CreateApplicationDefaultProxyCheckBoxLabel %>" />
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>      
            </wssuc:InputFormSection>                       
    </table>
</asp:Content>
                



