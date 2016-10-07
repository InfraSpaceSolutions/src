<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ConView.aspx.cs" Inherits="admin_ConView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="ConMenu" runat="server" Name="Contact Us Forms" Prefix="Con" Type="View" ShowAdd="false"/>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Label ID="ConIDLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Label ID="ConFNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Label ID="ConLNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email</td>
			<td class="formValue"><asp:HyperLink ID="ConEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">UTC Member?</td>
			<td class="formValue"><asp:Label ID="ConMemberLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">UTC Business Partner?</td>
			<td class="formValue"><asp:Label ID="ConPartnerLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Label ID="ConTimestampLabel" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this Contact Form?');" />
	</div>

	<h3>Message</h3>
	<hr />
	<div class="PublicPreview">
		<asp:Literal ID="ConCommentsLiteral" runat="server"></asp:Literal> 
	</div>
	<hr />
</asp:Content>

