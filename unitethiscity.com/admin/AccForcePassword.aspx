<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccForcePassword.aspx.cs" Inherits="admin_AccForcePassword" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="Force Password" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="AccIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email-Address</td>
			<td class="formValue"><asp:Literal ID="AccEMailLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Password<br />(Min. 6, Max. 20 characters)</td>
			<td class="formValue">
				<asp:TextBox ID="AccPasswordTextBox" runat="server" CssClass="formTextBox" MaxLength="50" Width="150" />
				<span class="note">Note: If a password is not supplied, one will be automatically generated.</span>
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
		<input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
	</div>
</asp:Content>

