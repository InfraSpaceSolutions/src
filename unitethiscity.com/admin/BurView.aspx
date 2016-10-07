<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BurView.aspx.cs" Inherits="admin_BurView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="BurMenu" runat="server" Name="Business Registration Forms" Prefix="Bur" Type="View" ShowAdd="false"/>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Label ID="BurIDLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Label ID="BurFNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Label ID="BurLNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Business Name</td>
			<td class="formValue"><asp:Label ID="BurBusinessNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Business Category</td>
			<td class="formValue"><asp:Label ID="BurCategoryLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email</td>
			<td class="formValue"><asp:HyperLink ID="BurEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Phone Number</td>
			<td class="formValue"><asp:Label ID="BurPhoneLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Label ID="BurTimestampLabel" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this Business Registration Form?');" />
	</div>

	<h3>Additional Information</h3>
	<hr />
	<div class="PublicPreview">
		<asp:Literal ID="BurAdditionalInfoLiteral" runat="server"></asp:Literal> 
	</div>
	<hr />
</asp:Content>

