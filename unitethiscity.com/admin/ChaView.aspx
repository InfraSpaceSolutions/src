<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ChaView.aspx.cs" Inherits="admin_ChaView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="ChaMenu" runat="server" Name="Charity Registration Forms" Prefix="Cha" Type="View" ShowAdd="false"/>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Label ID="ChaIDLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Label ID="ChaFNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Label ID="ChaLNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Charity Name</td>
			<td class="formValue"><asp:Label ID="ChaCharityNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email</td>
			<td class="formValue"><asp:HyperLink ID="ChaEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Phone Number</td>
			<td class="formValue"><asp:Label ID="ChaPhoneLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Label ID="ChaTimestampLabel" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this Charity Registration Form?');" />
	</div>

	<h3>Additional Information</h3>
	<hr />
	<div class="PublicPreview">
		<asp:Literal ID="ChaAdditionalInfoLiteral" runat="server"></asp:Literal> 
	</div>
	<hr />
</asp:Content>

