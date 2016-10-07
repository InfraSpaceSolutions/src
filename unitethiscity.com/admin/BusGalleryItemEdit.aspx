<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusGalleryItemEdit.aspx.cs" Inherits="admin_BusGalleryItemEdit" %>

<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" runat="Server">
    <menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="View" />
    <table class="formTable" cellspacing="1">
        <tr>
            <td class="formField">ID</td>
            <td class="formValue">
                <asp:Literal ID="BusIDLiteral" runat="server" /></td>
        </tr>
        <tr>
            <td class="formField">Guid</td>
            <td class="formValue">
                <asp:Literal ID="BusGuidLiteral" runat="server" /></td>
        </tr>
        <tr>
            <td class="formField">Name</td>
            <td class="formValue">
                <asp:HyperLink ID="BusNameHyperLink" runat="server" /></td>
        </tr>
        <tr>
            <td class="formField">Formal Name</td>
            <td class="formValue">
                <asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
        </tr>
        <tr>
            <td class="formField">Update Image</td>
            <td class="formValue">
                <asp:FileUpload ID="GalleryFileUpload" runat="server" />
            </td>
        </tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Upload" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
    </div>

    <br />
    <br />
    <div style="float: left; width: 45%; margin-right:5%;">
        <h3>Current Gallery Image</h3>
        <br />
        <asp:Image ID="GalleryImage" Style="width: 100%; height: auto;" BorderWidth="0" runat="server" />
    </div>
    <div style="float: left;">
        <h3>Auto-Cropped Thumbnail</h3>
        <br />
        <asp:Image ID="ThumbnailImage" BorderWidth="0" runat="server" />
    </div>
    <br style="clear:left;" />
    <br />
</asp:Content>